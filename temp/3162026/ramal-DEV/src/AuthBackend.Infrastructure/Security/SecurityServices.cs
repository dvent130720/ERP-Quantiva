using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthBackend.Application.Contracts;
using AuthBackend.Application.Models;
using AuthBackend.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthBackend.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    public JwtTokenService(IConfiguration configuration) => _configuration = configuration;

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("provider", user.Provider)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly IConfiguration _configuration;

    public GoogleTokenValidator(IConfiguration configuration) => _configuration = configuration;

    public Task<GooglePrincipal> ValidateIdTokenAsync(string idToken, CancellationToken ct = default)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
        var issuer = token.Issuer;
        var audience = token.Audiences.FirstOrDefault() ?? string.Empty;
        var exp = token.ValidTo;

        if (issuer is not ("https://accounts.google.com" or "accounts.google.com"))
            throw new UnauthorizedAccessException("Invalid issuer");

        if (audience != _configuration["GoogleOAuth:ClientId"])
            throw new UnauthorizedAccessException("Invalid audience");

        if (exp <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Expired id_token");

        var sub = token.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? throw new UnauthorizedAccessException("Missing sub");
        var email = token.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? throw new UnauthorizedAccessException("Missing email");

        return Task.FromResult(new GooglePrincipal(sub, email, issuer, audience, exp));
    }
}

public class RedisStateStore : IStateStore
{
    private readonly IDistributedCache _cache;
    public RedisStateStore(IDistributedCache cache) => _cache = cache;

    public async Task<bool> IsValidStateAsync(string state, CancellationToken ct = default)
    {
        var value = await _cache.GetStringAsync($"oauth_state:{state}", ct);
        if (value is null) return false;
        await _cache.RemoveAsync($"oauth_state:{state}", ct);
        return true;
    }

    public Task StoreStateAsync(string state, TimeSpan ttl, CancellationToken ct = default)
        => _cache.SetStringAsync($"oauth_state:{state}", "1", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, ct);
}
