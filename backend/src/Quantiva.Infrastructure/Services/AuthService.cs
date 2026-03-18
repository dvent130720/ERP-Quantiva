using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.Common.Exceptions;
using Quantiva.Application.DTOs;
using Quantiva.Infrastructure.Persistence;

namespace Quantiva.Infrastructure.Services;

public sealed class AuthService(AppDbContext dbContext, IPasswordHasher passwordHasher, ITokenService tokenService) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationAppException("Email y password son obligatorios.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var tenantSlug = string.IsNullOrWhiteSpace(request.TenantSlug) ? null : request.TenantSlug.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .Include(x => x.Tenant)
            .FirstOrDefaultAsync(
                x => x.Email == normalizedEmail &&
                    (tenantSlug == null ? x.IsPlatformAdmin : x.Tenant != null && x.Tenant.Slug == tenantSlug),
                cancellationToken);

        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        if (user.Tenant is not null && !user.Tenant.IsActive)
        {
            throw new ValidationAppException("El tenant asociado al usuario está inactivo.");
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var token = tokenService.CreateToken(user, user.Tenant?.Slug);
        var tenant = user.Tenant is null ? null : new TenantDto(user.Tenant.Id, user.Tenant.Name, user.Tenant.Slug, user.Tenant.PrimaryDomain, user.Tenant.ContactEmail, user.Tenant.IsActive);
        var profile = new UserProfileDto(user.Id, user.FullName, user.Email, user.IsPlatformAdmin);

        return new LoginResponseDto(token, profile, tenant);
    }
}
