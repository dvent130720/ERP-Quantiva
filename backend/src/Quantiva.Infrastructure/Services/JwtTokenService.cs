using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;
using Quantiva.Domain.Entities;
using Quantiva.Infrastructure.Security;

namespace Quantiva.Infrastructure.Services;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _options = options.Value;

    public AuthTokenDto CreateToken(AppUser user, string? tenantSlug)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.IsPlatformAdmin ? "PlatformAdmin" : "TenantAdmin")
        };

        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim("tenant_id", user.TenantId.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(tenantSlug))
        {
            claims.Add(new Claim("tenant_slug", tenantSlug));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthTokenDto(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
