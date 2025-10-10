// ============================================
// src/ERPSystem.API/Endpoints/Invoices/InvoiceEndpoints.cs
// ============================================
using Carter;
using Microsoft.EntityFrameworkCore;
using ERPSystem.Infrastructure.Persistence;
using ERPSystem.Domain.Entities.Invoices;
using ERPSystem.Infrastructure.Services.SRI;

namespace ERPSystem.API.Endpoints.Invoices;

public class InvoiceEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/invoices")
            .WithTags("Invoices");
            

        // GET: Listar todas las facturas
        group.MapGet("/", GetAllInvoices)
            .WithName("GetAllInvoices")
            .Produces<List<InvoiceDto>>(200);

        // GET: Obtener factura por ID
        group.MapGet("/{id:guid}", GetInvoiceById)
            .WithName("GetInvoiceById")
            .Produces<InvoiceDetailDto>(200)
            .Produces(404);

        // POST: Crear nueva factura
        group.MapPost("/", CreateInvoice)
            .WithName("CreateInvoice")
            .Produces<InvoiceDetailDto>(201)
            .Produces(400);

        // POST: Enviar factura al SRI
        group.MapPost("/{id:guid}/send-to-sri", SendInvoiceToSri)
            .WithName("SendInvoiceToSri")
            .Produces<SriResponseDto>(200)
            .Produces(404);

        // GET: Consultar estado de autorización
        group.MapGet("/{id:guid}/authorization-status", CheckAuthorizationStatus)
            .WithName("CheckAuthorizationStatus")
            .Produces<AuthorizationStatusDto>(200);
    }

    private static async Task<IResult> GetAllInvoices(
        ApplicationDbContext db,
        Guid? tenantId = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = db.Invoices
            .Include(i => i.Customer)
            .AsQueryable();

        if (tenantId.HasValue)
        {
            query = query.Where(i => i.TenantId == tenantId.Value);
        }

        var total = await query.CountAsync();
        var invoices = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                FullNumber = i.FullNumber,
                CustomerName = i.Customer.Name,
                IssueDate = i.IssueDate,
                Total = i.Total,
                Status = i.Status,
                AccessKey = i.AccessKey
            })
            .ToListAsync();

        return Results.Ok(new
        {
            data = invoices,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    private static async Task<IResult> GetInvoiceById(Guid id, ApplicationDbContext db)
    {
        var invoice = await db.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return Results.NotFound(new { message = "Factura no encontrada" });

        var dto = new InvoiceDetailDto
        {
            Id = invoice.Id,
            FullNumber = invoice.FullNumber,
            AccessKey = invoice.AccessKey,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            Customer = new CustomerDto
            {
                Id = invoice.Customer.Id,
                Name = invoice.Customer.Name,
                Identification = invoice.Customer.Identification,
                Email = invoice.Customer.Email,
                Address = invoice.Customer.Address
            },
            Details = invoice.Details.Select(d => new InvoiceDetailItemDto
            {
                ProductCode = d.ProductCode,
                ProductName = d.ProductName,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Discount = d.Discount,
                Subtotal = d.Subtotal,
                TaxAmount = d.TaxAmount,
                Total = d.Total
            }).ToList(),
            Subtotal = invoice.Subtotal,
            TaxAmount = invoice.TaxAmount,
            Discount = invoice.Discount,
            Total = invoice.Total,
            XmlContent = invoice.XmlContent,
            AuthorizationNumber = invoice.AuthorizationNumber,
            AuthorizationDate = invoice.AuthorizationDate
        };

        return Results.Ok(dto);
    }

    private static async Task<IResult> CreateInvoice(
        CreateInvoiceRequest request,
        ApplicationDbContext db,
        ISriInvoiceService sriService)
    {
        // Validar cliente
        var customer = await db.Customers.FindAsync(request.CustomerId);
        if (customer == null)
            return Results.BadRequest(new { message = "Cliente no encontrado" });

        // Validar productos
        var productIds = request.Details.Select(d => d.ProductId).ToList();
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        if (products.Count != productIds.Distinct().Count())
            return Results.BadRequest(new { message = "Uno o más productos no existen" });

        // Obtener siguiente secuencial
        var lastInvoice = await db.Invoices
            .Where(i => i.TenantId == request.TenantId)
            .OrderByDescending(i => i.SequentialNumber)
            .FirstOrDefaultAsync();

        var nextSequential = 1;
        if (lastInvoice != null && int.TryParse(lastInvoice.SequentialNumber, out var lastSeq))
        {
            nextSequential = lastSeq + 1;
        }

        // Crear factura
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            CustomerId = request.CustomerId,
            EstablishmentCode = "001",
            EmissionPointCode = "001",
            SequentialNumber = nextSequential.ToString("D9"),
            FullNumber = $"001-001-{nextSequential:D9}",
            IssueDate = DateTime.UtcNow,
            Status = "DRAFT"
        };

        // Crear detalles
        decimal subtotal = 0;
        decimal totalTax = 0;

        foreach (var detailRequest in request.Details)
        {
            var product = products[detailRequest.ProductId];
            var detailSubtotal = detailRequest.Quantity * detailRequest.UnitPrice - detailRequest.Discount;
            var detailTax = detailSubtotal * (product.TaxPercentage / 100);
            var detailTotal = detailSubtotal + detailTax;

            var detail = new InvoiceDetail
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                ProductId = product.Id,
                ProductCode = product.Code,
                ProductName = product.Name,
                Description = detailRequest.Description ?? product.Description,
                Quantity = detailRequest.Quantity,
                UnitPrice = detailRequest.UnitPrice,
                Discount = detailRequest.Discount,
                Subtotal = detailSubtotal,
                TaxPercentage = product.TaxPercentage,
                TaxAmount = detailTax,
                Total = detailTotal
            };

            invoice.Details.Add(detail);
            subtotal += detailSubtotal;
            totalTax += detailTax;
        }

        invoice.Subtotal = subtotal;
        invoice.TaxAmount = totalTax;
        invoice.Discount = request.Details.Sum(d => d.Discount);
        invoice.Total = subtotal + totalTax;

        db.Invoices.Add(invoice);
        await db.SaveChangesAsync();

        return Results.Created($"/api/invoices/{invoice.Id}", new { id = invoice.Id, fullNumber = invoice.FullNumber });
    }

    private static async Task<IResult> SendInvoiceToSri(
        Guid id,
        ApplicationDbContext db,
        ISriInvoiceService sriService)
    {
        var invoice = await db.Invoices.FindAsync(id);
        if (invoice == null)
            return Results.NotFound(new { message = "Factura no encontrada" });

        if (invoice.Status != "DRAFT")
            return Results.BadRequest(new { message = "Solo se pueden enviar facturas en estado DRAFT" });

        try
        {
            // Generar XML
            var xml = await sriService.GenerateXml(id);
            invoice.XmlContent = xml;
            invoice.Status = "PROCESSING";
            await db.SaveChangesAsync();

            // Firmar XML (por ahora omitido en pruebas)
            // var signedXml = await sriService.SignXml(xml, certificatePath, password);

            // Enviar al SRI
            var response = await sriService.SendToSri(xml);

            if (response.Success)
            {
                invoice.Status = "SENT";
                
                // Consultar autorización
                var authResponse = await sriService.CheckAuthorization(invoice.AccessKey!);
                if (authResponse.Success)
                {
                    invoice.Status = "AUTHORIZED";
                    invoice.AuthorizationNumber = authResponse.AuthorizationNumber;
                    invoice.AuthorizationDate = authResponse.AuthorizationDate;
                }
            }
            else
            {
                invoice.Status = "REJECTED";
                invoice.SriErrors = string.Join("; ", response.Errors);
            }

            await db.SaveChangesAsync();

            return Results.Ok(new SriResponseDto
            {
                Success = response.Success,
                Status = invoice.Status,
                Message = response.Message,
                AccessKey = invoice.AccessKey,
                AuthorizationNumber = invoice.AuthorizationNumber,
                Errors = response.Errors
            });
        }
        catch (Exception ex)
        {
            invoice.Status = "ERROR";
            invoice.SriErrors = ex.Message;
            await db.SaveChangesAsync();

            return Results.Ok(new SriResponseDto
            {
                Success = false,
                Status = "ERROR",
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
    }

    private static async Task<IResult> CheckAuthorizationStatus(
        Guid id,
        ApplicationDbContext db,
        ISriInvoiceService sriService)
    {
        var invoice = await db.Invoices.FindAsync(id);
        if (invoice == null)
            return Results.NotFound();

        if (string.IsNullOrEmpty(invoice.AccessKey))
            return Results.Ok(new AuthorizationStatusDto
            {
                Status = invoice.Status,
                Message = "Factura no enviada al SRI"
            });

        var response = await sriService.CheckAuthorization(invoice.AccessKey);

        return Results.Ok(new AuthorizationStatusDto
        {
            Status = response.State ?? invoice.Status,
            AuthorizationNumber = response.AuthorizationNumber,
            AuthorizationDate = response.AuthorizationDate,
            Message = response.Success ? "Autorizado" : "No autorizado",
            Errors = response.Errors
        });
    }
}

// ============================================
// DTOs
// ============================================
public record InvoiceDto
{
    public Guid Id { get; init; }
    public string FullNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public decimal Total { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? AccessKey { get; init; }
}

public record InvoiceDetailDto
{
    public Guid Id { get; init; }
    public string FullNumber { get; init; } = string.Empty;
    public string? AccessKey { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public CustomerDto Customer { get; init; } = null!;
    public List<InvoiceDetailItemDto> Details { get; init; } = new();
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Discount { get; init; }
    public decimal Total { get; init; }
    public string? XmlContent { get; init; }
    public string? AuthorizationNumber { get; init; }
    public DateTime? AuthorizationDate { get; init; }
}

public record CustomerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Identification { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

public record InvoiceDetailItemDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Discount { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Total { get; init; }
}

public record CreateInvoiceRequest
{
    public Guid TenantId { get; init; }
    public Guid CustomerId { get; init; }
    public List<CreateInvoiceDetailRequest> Details { get; init; } = new();
}

public record CreateInvoiceDetailRequest
{
    public Guid ProductId { get; init; }
    public string? Description { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Discount { get; init; }
}

public record SriResponseDto
{
    public bool Success { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Message { get; init; }
    public string? AccessKey { get; init; }
    public string? AuthorizationNumber { get; init; }
    public List<string> Errors { get; init; } = new();
}

public record AuthorizationStatusDto
{
    public string Status { get; init; } = string.Empty;
    public string? AuthorizationNumber { get; init; }
    public DateTime? AuthorizationDate { get; init; }
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = new();
}