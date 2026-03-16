using AuthBackend.Application.Models;
using AuthBackend.Domain.Entities;

namespace AuthBackend.Application.Contracts;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}

public interface IGoogleTokenValidator
{
    Task<GooglePrincipal> ValidateIdTokenAsync(string idToken, CancellationToken ct = default);
}

public interface IStateStore
{
    Task<bool> IsValidStateAsync(string state, CancellationToken ct = default);
    Task StoreStateAsync(string state, TimeSpan ttl, CancellationToken ct = default);
}
