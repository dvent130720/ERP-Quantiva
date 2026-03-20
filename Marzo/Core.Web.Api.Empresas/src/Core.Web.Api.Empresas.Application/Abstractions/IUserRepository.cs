using Core.Web.Api.Empresas.Domain.Entities;

namespace Core.Web.Api.Empresas.Application.Abstractions;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
}
