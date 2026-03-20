using Core.Web.Api.Empresas.Domain.Entities;

namespace Core.Web.Api.Empresas.Application.Abstractions;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) Generate(ApplicationUser user);
}
