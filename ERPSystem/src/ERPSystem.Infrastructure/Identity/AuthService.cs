using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ERPSystem.Infrastructure.Persistence;
using ERPSystem.Domain.Entities.Users;

namespace ERPSystem.Infrastructure.Services.Identity;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> Login(string username, string password)
    {
        var user = await _context.Users
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null)
            return null;

        // Verificar contraseña
        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        // Actualizar último login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generar token
        var token = GenerateJwtToken(user.Id, user.Username, user.Email, user.Role, user.TenantId);

        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "480");
        
        return new AuthResponse(
            Token: token,
            Username: user.Username,
            Email: user.Email,
            Role: user.Role,
            UserId: user.Id,
            TenantId: user.TenantId,
            ExpiresAt: DateTime.UtcNow.AddMinutes(expirationMinutes)
        );
    }

    public async Task<AuthResponse?> Register(RegisterRequest request)
    {
        // Verificar que el tenant existe
        var tenant = await _context.Tenants.FindAsync(request.TenantId);
        if (tenant == null || !tenant.IsActive)
            return null;

        // Verificar que el username no exista
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

        if (existingUser != null)
            return null;

        // Crear usuario
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generar token
        var token = GenerateJwtToken(user.Id, user.Username, user.Email, user.Role, user.TenantId);

        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "480");

        return new AuthResponse(
            Token: token,
            Username: user.Username,
            Email: user.Email,
            Role: user.Role,
            UserId: user.Id,
            TenantId: user.TenantId,
            ExpiresAt: DateTime.UtcNow.AddMinutes(expirationMinutes)
        );
    }

    public string GenerateJwtToken(Guid userId, string username, string email, string role, Guid tenantId)
    {
        var secret = _configuration["JwtSettings:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret not configured");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("TenantId", tenantId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "480");

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        // Usar BCrypt o similar en producción
        // Por ahora usamos SHA256 como ejemplo
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }
}