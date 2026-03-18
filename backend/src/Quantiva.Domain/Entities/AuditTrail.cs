using Quantiva.Domain.Common;

namespace Quantiva.Domain.Entities;

public sealed class AuditTrail : BaseEntity
{
    public Guid? TenantId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public string? SourceIp { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public string ChangesJson { get; set; } = string.Empty;
}
