using Quantiva.Application.Common.Abstractions;

namespace Quantiva.Infrastructure.Services;

public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; private set; }
    public string? TenantSlug { get; private set; }
    public string? TenantName { get; private set; }
    public bool IsAvailable => TenantId.HasValue;

    public void Set(Guid tenantId, string tenantSlug, string tenantName)
    {
        TenantId = tenantId;
        TenantSlug = tenantSlug;
        TenantName = tenantName;
    }

    public void Clear()
    {
        TenantId = null;
        TenantSlug = null;
        TenantName = null;
    }
}
