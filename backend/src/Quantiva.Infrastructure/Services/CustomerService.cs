using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;
using Quantiva.Domain.Entities;
using Quantiva.Infrastructure.Persistence;

namespace Quantiva.Infrastructure.Services;

public sealed class CustomerService(AppDbContext dbContext, ITenantContext tenantContext) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        return await dbContext.Customers
            .AsNoTracking()
            .OrderBy(x => x.LegalName)
            .Select(x => new CustomerDto(x.Id, x.Code, x.LegalName, x.ContactEmail, x.ContactPhone, x.Notes, x.IsActive, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        return await dbContext.Customers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CustomerDto(x.Id, x.Code, x.LegalName, x.ContactEmail, x.ContactPhone, x.Notes, x.IsActive, x.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CustomerDto> CreateAsync(UpsertCustomerDto request, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        var entity = new Customer
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            LegalName = request.LegalName.Trim(),
            ContactEmail = request.ContactEmail.Trim().ToLowerInvariant(),
            ContactPhone = request.ContactPhone?.Trim(),
            Notes = request.Notes?.Trim(),
            IsActive = request.IsActive
        };

        dbContext.Customers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CustomerDto(entity.Id, entity.Code, entity.LegalName, entity.ContactEmail, entity.ContactPhone, entity.Notes, entity.IsActive, entity.UpdatedAtUtc);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpsertCustomerDto request, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        var entity = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Code = request.Code.Trim().ToUpperInvariant();
        entity.LegalName = request.LegalName.Trim();
        entity.ContactEmail = request.ContactEmail.Trim().ToLowerInvariant();
        entity.ContactPhone = request.ContactPhone?.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CustomerDto(entity.Id, entity.Code, entity.LegalName, entity.ContactEmail, entity.ContactPhone, entity.Notes, entity.IsActive, entity.UpdatedAtUtc);
    }

    private void EnsureTenant()
    {
        if (!tenantContext.IsAvailable)
        {
            throw new InvalidOperationException("Tenant no resuelto. Envía X-Tenant-Slug o usa un dominio configurado.");
        }
    }
}
