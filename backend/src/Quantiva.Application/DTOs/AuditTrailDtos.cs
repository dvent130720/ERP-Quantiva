namespace Quantiva.Application.DTOs;

public sealed record AuditTrailDto(Guid Id, Guid? TenantId, string EntityName, string EntityId, string Action, string? UserEmail, string? SourceIp, DateTime OccurredAtUtc, string ChangesJson);
