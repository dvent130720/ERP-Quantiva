using Carter;
using Microsoft.EntityFrameworkCore;
using ERPSystem.Infrastructure.Persistence;
using ERPSystem.Domain.Entities.Customers;

namespace ERPSystem.API.Endpoints.Customers;

public class CustomerEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers").WithTags("Customers");

        group.MapGet("/", async (ApplicationDbContext db, Guid? tenantId) =>
        {
            var query = db.Customers.AsQueryable();
            if (tenantId.HasValue)
                query = query.Where(c => c.TenantId == tenantId.Value);

            var customers = await query
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Results.Ok(customers);
        });

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext db) =>
        {
            var customer = await db.Customers.FindAsync(id);
            return customer is not null ? Results.Ok(customer) : Results.NotFound();
        });

        group.MapPost("/", async (CreateCustomerRequest request, ApplicationDbContext db) =>
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId,
                IdentificationType = request.IdentificationType,
                Identification = request.Identification,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address
            };

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            return Results.Created($"/api/customers/{customer.Id}", customer);
        });
    }
}

public record CreateCustomerRequest(
    Guid TenantId,
    string IdentificationType,
    string Identification,
    string Name,
    string Email,
    string Phone,
    string Address
);