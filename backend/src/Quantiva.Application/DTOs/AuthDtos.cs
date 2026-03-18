namespace Quantiva.Application.DTOs;

public sealed record LoginRequestDto(string Email, string Password, string? TenantSlug);
public sealed record AuthTokenDto(string AccessToken, DateTime ExpiresAtUtc);
public sealed record LoginResponseDto(AuthTokenDto Token, UserProfileDto User, TenantDto? Tenant);
public sealed record UserProfileDto(Guid Id, string FullName, string Email, bool IsPlatformAdmin);
