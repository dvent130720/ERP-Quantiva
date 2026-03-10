using Carter;
using ERPSystem.API.Services;
using ERPSystem.Domain.Entities.Invoices;
using ERPSystem.Infrastructure.Persistence;
using ERPSystem.Infrastructure.Services.SRI;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.API.Endpoints.ElectronicBilling;

public class ElectronicBillingEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/electronic-billing").WithTags("ElectronicBilling");

        group.MapPost("/receive-invoice", ReceiveInvoiceAsync)
            .WithName("ReceiveInvoice")
            .Produces(202)
            .Produces(400);
    }

    private static async Task<IResult> ReceiveInvoiceAsync(
        ReceiveInvoiceRequest request,
        ApplicationDbContext db,
        ISriInvoiceService sriService,
        IInvoiceDispatchQueue queue)
    {
        var customer = await db.Customers.FindAsync(request.CustomerId);
        if (customer is null)
            return Results.BadRequest(new { message = "Cliente no existe" });

        var invoice = await db.Invoices
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId);

        if (invoice is null)
            return Results.BadRequest(new { message = "Factura no existe" });

        if (invoice.Status is "AUTHORIZED" or "SENT")
            return Results.Ok(new { message = "Factura ya procesada", invoice.Status, invoice.Id });

        var xml = await sriService.GenerateXml(invoice.Id);
        var sriResult = await sriService.SendToSri(xml);

        invoice.Status = sriResult.Success ? "SENT" : "REJECTED";
        invoice.SriResponse = sriResult.Message;
        invoice.SriErrors = sriResult.Success ? null : string.Join(";", sriResult.Errors);
        invoice.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        if (sriResult.Success)
            await queue.EnqueueAsync(invoice.Id);

        return Results.Accepted($"/api/invoices/{invoice.Id}", new
        {
            invoice.Id,
            invoice.FullNumber,
            invoice.Status,
            message = sriResult.Message,
            errors = sriResult.Errors
        });
    }
}

public record ReceiveInvoiceRequest(Guid InvoiceId, Guid CustomerId);
