using Quantiva.Domain.Common;

namespace Quantiva.Domain.Entities;

public sealed class Customer : TenantScopedEntity
{
    public Tenant Tenant { get; set; } = null!;
    public string Code { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}
