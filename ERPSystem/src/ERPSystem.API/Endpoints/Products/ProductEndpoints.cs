using Carter;
using Microsoft.EntityFrameworkCore;
using ERPSystem.Infrastructure.Persistence;
using ERPSystem.Domain.Entities.Products;

namespace ERPSystem.API.Endpoints.Products;

public class ProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (ApplicationDbContext db, Guid? tenantId) =>
        {
            var query = db.Products.AsQueryable();
            if (tenantId.HasValue)
                query = query.Where(p => p.TenantId == tenantId.Value);

            var products = await query
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return Results.Ok(products);
        });

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });

        group.MapPost("/", async (CreateProductRequest request, ApplicationDbContext db) =>
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Cost = request.Cost,
                TaxType = request.TaxType,
                TaxPercentage = request.TaxPercentage,
                StockQuantity = request.StockQuantity
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            return Results.Created($"/api/products/{product.Id}", product);
        });
    }
}

public record CreateProductRequest(
    Guid TenantId,
    string Code,
    string Name,
    string Description,
    decimal Price,
    decimal Cost,
    string TaxType,
    decimal TaxPercentage,
    int StockQuantity
);