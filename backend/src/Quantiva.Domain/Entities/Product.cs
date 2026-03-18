using Quantiva.Domain.Common;

namespace Quantiva.Domain.Entities;

public sealed class Product : TenantScopedEntity
{
    public Tenant Tenant { get; set; } = null!;
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}
