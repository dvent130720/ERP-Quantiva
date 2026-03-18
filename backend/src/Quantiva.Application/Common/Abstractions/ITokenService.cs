using Quantiva.Application.DTOs;
using Quantiva.Domain.Entities;

namespace Quantiva.Application.Common.Abstractions;

public interface ITokenService
{
    AuthTokenDto CreateToken(AppUser user, string? tenantSlug);
}
