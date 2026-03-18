using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;
using Quantiva.Domain.Entities;
using Quantiva.Infrastructure.Persistence;

namespace Quantiva.Infrastructure.Services;

public sealed class ProductService(AppDbContext dbContext, ITenantContext tenantContext) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto(x.Id, x.Sku, x.Name, x.Description, x.Price, x.IsActive, x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductDto> CreateAsync(UpsertProductDto request, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        var entity = new Product
        {
            Sku = request.Sku.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            IsActive = request.IsActive
        };

        dbContext.Products.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ProductDto(entity.Id, entity.Sku, entity.Name, entity.Description, entity.Price, entity.IsActive, entity.UpdatedAtUtc);
    }

    private void EnsureTenant()
    {
        if (!tenantContext.IsAvailable)
        {
            throw new InvalidOperationException("Tenant no resuelto. Envía X-Tenant-Slug o usa un dominio configurado.");
        }
    }
}
