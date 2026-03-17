using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/identity/token", (LoginRequest request, IConfiguration config) =>
{
    var secret = config["Jwt:Secret"] ?? "dev-secret-change-me-dev-secret-change-me";
    var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256);
    var claims = new[]
    {
        new Claim("tenant_id", request.TenantId),
        new Claim(ClaimTypes.Name, request.UserName),
        new Claim(ClaimTypes.Role, "admin")
    };

    var token = new JwtSecurityToken(
        issuer: "quantiva.identity",
        audience: "quantiva.gateway",
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(20),
        signingCredentials: credentials);

    return Results.Ok(new
    {
        access_token = new JwtSecurityTokenHandler().WriteToken(token),
        refresh_token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "identity" }));
app.Run();

public sealed record LoginRequest(string UserName, string Password, string TenantId);
