namespace Quantiva.Application.DTOs;

public sealed record CustomerDto(Guid Id, string Code, string LegalName, string ContactEmail, string? ContactPhone, string? Notes, bool IsActive, DateTime UpdatedAtUtc);
public sealed record UpsertCustomerDto(string Code, string LegalName, string ContactEmail, string? ContactPhone, string? Notes, bool IsActive);
