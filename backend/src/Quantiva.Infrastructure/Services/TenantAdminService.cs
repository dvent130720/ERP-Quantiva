using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.Common.Exceptions;
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
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Slug))
        {
            throw new ValidationAppException("Name y Slug son obligatorios para crear un tenant.");
        }

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();
        var normalizedDomain = request.PrimaryDomain?.Trim().ToLowerInvariant();
        var normalizedAdminEmail = request.AdminEmail.Trim().ToLowerInvariant();

        if (await dbContext.Tenants.AnyAsync(x => x.Slug == normalizedSlug, cancellationToken))
        {
            throw new ConflictAppException($"Ya existe un tenant con slug {normalizedSlug}.");
        }

        if (!string.IsNullOrWhiteSpace(normalizedDomain) && await dbContext.Tenants.AnyAsync(x => x.PrimaryDomain == normalizedDomain, cancellationToken))
        {
            throw new ConflictAppException($"El dominio {normalizedDomain} ya está asignado a otro tenant.");
        }

        if (await dbContext.Users.AnyAsync(x => x.Email == normalizedAdminEmail && x.TenantId != null, cancellationToken))
        {
            throw new ConflictAppException($"Ya existe un usuario tenant con email {normalizedAdminEmail}.");
        }

        var tenant = new Tenant
        {
            Name = request.Name.Trim(),
            Slug = normalizedSlug,
            PrimaryDomain = normalizedDomain,
            ContactEmail = request.ContactEmail?.Trim().ToLowerInvariant()
        };

        var adminUser = new AppUser
        {
            Tenant = tenant,
            FullName = request.AdminFullName.Trim(),
            Email = normalizedAdminEmail,
            PasswordHash = passwordHasher.Hash(request.AdminPassword)
        };

        dbContext.Tenants.Add(tenant);
        dbContext.Users.Add(adminUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TenantDto(tenant.Id, tenant.Name, tenant.Slug, tenant.PrimaryDomain, tenant.ContactEmail, tenant.IsActive);
    }
}
