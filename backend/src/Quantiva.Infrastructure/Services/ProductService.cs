using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.Common.Exceptions;
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
            .Select(Map())
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        var product = await dbContext.Products
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(Map())
            .FirstOrDefaultAsync(cancellationToken);

        return product ?? throw new NotFoundAppException("El producto solicitado no existe para el tenant actual.");
    }

    public async Task<ProductDto> CreateAsync(UpsertProductDto request, CancellationToken cancellationToken = default)
    {
        EnsureTenant();
        Validate(request);

        var sku = request.Sku.Trim().ToUpperInvariant();
        var exists = await dbContext.Products.AnyAsync(x => x.Sku == sku, cancellationToken);
        if (exists)
        {
            throw new ConflictAppException($"Ya existe un producto con SKU {sku} en este tenant.");
        }

        var entity = new Product
        {
            Sku = sku,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            IsActive = request.IsActive
        };

        dbContext.Products.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpsertProductDto request, CancellationToken cancellationToken = default)
    {
        EnsureTenant();
        Validate(request);

        var entity = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException("El producto solicitado no existe para el tenant actual.");

        var sku = request.Sku.Trim().ToUpperInvariant();
        var duplicate = await dbContext.Products.AnyAsync(x => x.Id != id && x.Sku == sku, cancellationToken);
        if (duplicate)
        {
            throw new ConflictAppException($"Ya existe otro producto con SKU {sku} en este tenant.");
        }

        entity.Sku = sku;
        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.Price = request.Price;
        entity.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureTenant();

        var entity = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundAppException("El producto solicitado no existe para el tenant actual.");

        dbContext.Products.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void EnsureTenant()
    {
        if (!tenantContext.IsAvailable)
        {
            throw new TenantResolutionException("Tenant no resuelto. Envía X-Tenant-Slug o usa un dominio configurado.");
        }
    }

    private static void Validate(UpsertProductDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
        {
            throw new ValidationAppException("Sku, Name y Price deben ser válidos.");
        }
    }

    private static System.Linq.Expressions.Expression<Func<Product, ProductDto>> Map() =>
        x => new ProductDto(x.Id, x.Sku, x.Name, x.Description, x.Price, x.IsActive, x.UpdatedAtUtc);

    private static ProductDto ToDto(Product x) => new(x.Id, x.Sku, x.Name, x.Description, x.Price, x.IsActive, x.UpdatedAtUtc);
}
