namespace Quantiva.Application.DTOs;

public sealed record TenantDto(Guid Id, string Name, string Slug, string? PrimaryDomain, string? ContactEmail, bool IsActive);
public sealed record CreateTenantDto(string Name, string Slug, string? PrimaryDomain, string? ContactEmail, string AdminFullName, string AdminEmail, string AdminPassword);
