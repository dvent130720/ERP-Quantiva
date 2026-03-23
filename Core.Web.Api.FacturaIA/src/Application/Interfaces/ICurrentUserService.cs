namespace Core.Web.Api.FacturaIA.Application.Interfaces;

public interface ICurrentUserService
{
    Guid GetTenantId();
    string GetUserName();
}
