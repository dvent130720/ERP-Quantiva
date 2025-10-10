public interface IAuthService
{
    Task<AuthResponse?> Login(string username, string password);
    Task<AuthResponse?> Register(RegisterRequest request);
    string GenerateJwtToken(Guid userId, string username, string email, string role, Guid tenantId);
}

public record AuthResponse(
    string Token,
    string Username,
    string Email,
    string Role,
    Guid UserId,
    Guid TenantId,
    DateTime ExpiresAt
);

public record RegisterRequest(
    Guid TenantId,
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role = "USER"
);