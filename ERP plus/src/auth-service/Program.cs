using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MultiTenant;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().WriteTo.Console().WriteTo.Seq(builder.Configuration["Serilog:SeqUrl"] ?? "http://seq:5341").CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddCommonPlatform(builder.Configuration, "auth-service");
builder.Services.AddCommonHealthChecks(builder.Configuration);

var app = builder.Build();
app.UseCommonPlatform();
app.MapHealthChecks("/health");

app.MapPost("/api/auth/token", (LoginRequest req, IConfiguration config) =>
{
    var claims = new[]
    {
        new Claim("user_id", req.UserId),
        new Claim("tenant_id", req.TenantId),
        new Claim("role", req.Role),
        new Claim(ClaimTypes.Role, req.Role)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: creds);
    return Results.Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token) });
}).AllowAnonymous();

app.Run();

public sealed record LoginRequest(string UserId, string TenantId, string Role);
