using System.Security.Claims;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Shared.Constants;
using Microsoft.AspNetCore.Http;

namespace Core.Web.Api.FacturaIA.Infrastructure.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimConstants.TenantId);
        if (!Guid.TryParse(value, out var tenantId))
        {
            throw new UnauthorizedAccessException("El claim tenant_id es obligatorio.");
        }

        return tenantId;
    }

    public string GetUserName()
        => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimConstants.UserName)
           ?? _httpContextAccessor.HttpContext?.User.Identity?.Name
           ?? "anonymous";
}
