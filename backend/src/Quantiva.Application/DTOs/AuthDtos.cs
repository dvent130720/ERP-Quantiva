using System.ComponentModel.DataAnnotations;

namespace Quantiva.Application.DTOs;

public sealed record LoginRequestDto(
    [property: Required, EmailAddress, MaxLength(180)] string Email,
    [property: Required, MinLength(8), MaxLength(120)] string Password,
    [property: MaxLength(80)] string? TenantSlug);

public sealed record AuthTokenDto(string AccessToken, DateTime ExpiresAtUtc);
public sealed record LoginResponseDto(AuthTokenDto Token, UserProfileDto User, TenantDto? Tenant);
public sealed record UserProfileDto(Guid Id, string FullName, string Email, bool IsPlatformAdmin);
