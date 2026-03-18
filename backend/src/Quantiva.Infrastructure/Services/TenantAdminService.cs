using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;
using Quantiva.Domain.Entities;
using Quantiva.Infrastructure.Persistence;

namespace Quantiva.Infrastructure.Services;

public sealed class TenantAdminService(AppDbContext dbContext, IPasswordHasher passwordHasher) : ITenantAdminService
{
    public async Task<IReadOnlyList<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Tenants
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new TenantDto(x.Id, x.Name, x.Slug, x.PrimaryDomain, x.ContactEmail, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<TenantDto> CreateAsync(CreateTenantDto request, CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Slug = request.Slug.Trim().ToLowerInvariant(),
            PrimaryDomain = request.PrimaryDomain?.Trim().ToLowerInvariant(),
            ContactEmail = request.ContactEmail?.Trim().ToLowerInvariant()
        };

        var adminUser = new AppUser
        {
            Tenant = tenant,
            FullName = request.AdminFullName.Trim(),
            Email = request.AdminEmail.Trim().ToLowerInvariant(),
            PasswordHash = passwordHasher.Hash(request.AdminPassword)
        };

        dbContext.Tenants.Add(tenant);
        dbContext.Users.Add(adminUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.PrimaryDomain, tenant.ContactEmail, tenant.IsActive);
    }
}
