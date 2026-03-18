using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.Common.Exceptions;
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
            .Select(Map())
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        var customer = await dbContext.Customers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(Map())
            .FirstOrDefaultAsync(cancellationToken);

        return customer ?? throw new NotFoundAppException("El cliente solicitado no existe para el tenant actual.");
    }

    public async Task<CustomerDto> CreateAsync(UpsertCustomerDto request, CancellationToken cancellationToken = default)
    {
        EnsureTenant();
        Validate(request);

        var code = request.Code.Trim().ToUpperInvariant();
        var email = request.ContactEmail.Trim().ToLowerInvariant();

        var exists = await dbContext.Customers.AnyAsync(x => x.Code == code, cancellationToken);
        if (exists)
        {
            throw new ConflictAppException($"Ya existe un cliente con código {code} en este tenant.");
        }

        var entity = new Customer
        {
            Code = code,
            LegalName = request.LegalName.Trim(),
            ContactEmail = email,
            ContactPhone = request.ContactPhone?.Trim(),
            Notes = request.Notes?.Trim(),
            IsActive = request.IsActive
        };

        dbContext.Customers.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task<CustomerDto> UpdateAsync(Guid id, UpsertCustomerDto request, CancellationToken cancellationToken = default)
    {
        EnsureTenant();
        Validate(request);

        var entity = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException("El cliente solicitado no existe para el tenant actual.");

        var code = request.Code.Trim().ToUpperInvariant();
        var duplicate = await dbContext.Customers.AnyAsync(x => x.Id != id && x.Code == code, cancellationToken);
        if (duplicate)
        {
            throw new ConflictAppException($"Ya existe otro cliente con código {code} en este tenant.");
        }

        entity.Code = code;
        entity.LegalName = request.LegalName.Trim();
        entity.ContactEmail = request.ContactEmail.Trim().ToLowerInvariant();
        entity.ContactPhone = request.ContactPhone?.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        var entity = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException("El cliente solicitado no existe para el tenant actual.");

        dbContext.Customers.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void EnsureTenant()
    {
        if (!tenantContext.IsAvailable)
        {
            throw new TenantResolutionException("Tenant no resuelto. Envía X-Tenant-Slug o usa un dominio configurado.");
        }
    }

    private static void Validate(UpsertCustomerDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.LegalName) || string.IsNullOrWhiteSpace(request.ContactEmail))
        {
            throw new ValidationAppException("Code, LegalName y ContactEmail son obligatorios.");
        }
    }

    private static System.Linq.Expressions.Expression<Func<Customer, CustomerDto>> Map() =>
        x => new CustomerDto(x.Id, x.Code, x.LegalName, x.ContactEmail, x.ContactPhone, x.Notes, x.IsActive, x.UpdatedAtUtc);

    private static CustomerDto ToDto(Customer x) => new(x.Id, x.Code, x.LegalName, x.ContactEmail, x.ContactPhone, x.Notes, x.IsActive, x.UpdatedAtUtc);
}
