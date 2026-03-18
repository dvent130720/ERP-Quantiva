using System.ComponentModel.DataAnnotations;

namespace Quantiva.Application.DTOs;

public sealed record TenantDto(Guid Id, string Name, string Slug, string? PrimaryDomain, string? ContactEmail, bool IsActive);

public sealed record CreateTenantDto(
    [property: Required, MaxLength(120)] string Name,
    [property: Required, MaxLength(80)] string Slug,
    [property: MaxLength(180)] string? PrimaryDomain,
    [property: EmailAddress, MaxLength(180)] string? ContactEmail,
    [property: Required, MaxLength(140)] string AdminFullName,
    [property: Required, EmailAddress, MaxLength(180)] string AdminEmail,
    [property: Required, MinLength(8), MaxLength(120)] string AdminPassword);
