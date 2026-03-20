namespace Core.Web.Api.Empresas.Application.DTOs;

public sealed record CompanyRequest(
    string TaxId,
    string LegalName,
    string? TradeName,
    string Email,
    string? Phone,
    string? Address,
    bool IsActive);

public sealed record CompanyResponse(
    Guid Id,
    string TaxId,
    string LegalName,
    string? TradeName,
    string Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record LoginRequest(string Username, string Password);

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, string TokenType, string Role);
