namespace AuthBackend.Application.Models;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password);
public record GoogleCallbackRequest(string IdToken, string State);
public record RefreshRequest(string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
public record GooglePrincipal(string Subject, string Email, string Issuer, string Audience, DateTime ExpiresAtUtc);
