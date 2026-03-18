namespace Quantiva.Application.Common.Abstractions;

public interface ITenantContext
{
    Guid? TenantId { get; }
    string? TenantSlug { get; }
    string? TenantName { get; }
    bool IsAvailable { get; }
    void Set(Guid tenantId, string tenantSlug, string tenantName);
    void Clear();
}
