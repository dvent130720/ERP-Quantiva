namespace Core.Web.Api.Empresas.Application.Abstractions;

public interface IPasswordHasher
{
    bool Verify(string password, string passwordHash);
}
