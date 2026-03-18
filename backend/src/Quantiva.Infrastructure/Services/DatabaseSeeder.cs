using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Domain.Entities;
using Quantiva.Infrastructure.Persistence;

namespace Quantiva.Infrastructure.Services;

public sealed class DatabaseSeeder(AppDbContext dbContext, IPasswordHasher passwordHasher)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Tenants.AnyAsync(cancellationToken))
        {
            return;
        }

        var tenant = new Tenant
        {
            Name = "Quantiva Demo",
            Slug = "quantiva-demo",
            PrimaryDomain = "demo.quantiva-solutions.com",
            ContactEmail = "admin@quantiva-solutions.com",
            CreatedBy = "seed",
            UpdatedBy = "seed"
        };

        var platformAdmin = new AppUser
        {
            FullName = "Platform Admin",
            Email = "platform@quantiva-solutions.com",
            PasswordHash = passwordHasher.Hash("ChangeMe123!"),
            IsPlatformAdmin = true,
            CreatedBy = "seed",
            UpdatedBy = "seed"
        };

        var tenantAdmin = new AppUser
        {
            Tenant = tenant,
            FullName = "Tenant Admin",
            Email = "admin@quantiva-solutions.com",
            PasswordHash = passwordHasher.Hash("ChangeMe123!"),
            IsPlatformAdmin = false,
            CreatedBy = "seed",
            UpdatedBy = "seed"
        };

        dbContext.Tenants.Add(tenant);
        dbContext.Users.AddRange(platformAdmin, tenantAdmin);
        dbContext.Customers.Add(new Customer
        {
            Tenant = tenant,
            Code = "CLI-001",
            LegalName = "Cliente Inicial S.A.S.",
            ContactEmail = "cliente@ejemplo.com",
            ContactPhone = "+57 3000000000",
            Notes = "Registro semilla para validar el multi-tenant.",
            CreatedBy = "seed",
            UpdatedBy = "seed"
        });
        dbContext.Products.Add(new Product
        {
            Tenant = tenant,
            Sku = "ERP-PLAN-PRO",
            Name = "Plan ERP Pro",
            Description = "Producto inicial de catálogo.",
            Price = 199.00m,
            CreatedBy = "seed",
            UpdatedBy = "seed"
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
