using Carter;
using Microsoft.EntityFrameworkCore;
using ERPSystem.Infrastructure.Persistence;
using ERPSystem.Domain.Entities.Tenants;

namespace ERPSystem.API.Endpoints.Tenants;

public class TenantEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenants").WithTags("Tenants");

        // GET: Listar todas las empresas
        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var tenants = await db.Tenants
                .Where(t => t.IsActive)
                .OrderBy(t => t.CompanyName)
                .ToListAsync();

            return Results.Ok(tenants);
        })
        .WithName("GetAllTenants")
        .Produces<List<Tenant>>(200);

        // GET: Obtener empresa por ID
        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            return tenant is not null ? Results.Ok(tenant) : Results.NotFound();
        })
        .WithName("GetTenantById")
        .Produces<Tenant>(200)
        .Produces(404);

        // POST: Crear nueva empresa
        group.MapPost("/", async (CreateTenantRequest request, ApplicationDbContext db) =>
        {
            // Validar que el RUC no exista
            var existingTenant = await db.Tenants
                .FirstOrDefaultAsync(t => t.Ruc == request.Ruc);

            if (existingTenant != null)
            {
                return Results.BadRequest(new { message = "Ya existe una empresa con este RUC" });
            }

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                CompanyName = request.CompanyName,
                CommercialName = request.CommercialName,
                Ruc = request.Ruc,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                SriEnvironment = request.SriEnvironment ?? "PRUEBAS",
                SubscriptionPlan = "FREE",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();

            return Results.Created($"/api/tenants/{tenant.Id}", tenant);
        })
        .WithName("CreateTenant")
        .Produces<Tenant>(201)
        .Produces(400);

        // PUT: Actualizar empresa
        group.MapPut("/{id:guid}", async (Guid id, UpdateTenantRequest request, ApplicationDbContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return Results.NotFound(new { message = "Empresa no encontrada" });
            }

            tenant.CompanyName = request.CompanyName ?? tenant.CompanyName;
            tenant.CommercialName = request.CommercialName ?? tenant.CommercialName;
            tenant.Email = request.Email ?? tenant.Email;
            tenant.Phone = request.Phone ?? tenant.Phone;
            tenant.Address = request.Address ?? tenant.Address;
            tenant.SriEnvironment = request.SriEnvironment ?? tenant.SriEnvironment;
            tenant.ElectronicSignaturePath = request.ElectronicSignaturePath ?? tenant.ElectronicSignaturePath;
            tenant.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(tenant);
        })
        .WithName("UpdateTenant")
        .Produces<Tenant>(200)
        .Produces(404);

        // DELETE: Desactivar empresa (soft delete)
        group.MapDelete("/{id:guid}", async (Guid id, ApplicationDbContext db) =>
        {
            var tenant = await db.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return Results.NotFound();
            }

            tenant.IsActive = false;
            tenant.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("DeleteTenant")
        .Produces(204)
        .Produces(404);
    }
}

// ============================================
// DTOs
// ============================================
public record CreateTenantRequest(
    string CompanyName,
    string CommercialName,
    string Ruc,
    string Email,
    string Phone,
    string Address,
    string? SriEnvironment
);

public record UpdateTenantRequest(
    string? CompanyName,
    string? CommercialName,
    string? Email,
    string? Phone,
    string? Address,
    string? SriEnvironment,
    string? ElectronicSignaturePath
);