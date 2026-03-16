using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MultiTenant;

public interface ITenantProvider
{
    TenantContext Current { get; }
}

public sealed class TenantProvider(IHttpContextAccessor accessor) : ITenantProvider
{
    public TenantContext Current
    {
        get
        {
            var context = accessor.HttpContext;
            if (context is null)
            {
                return new TenantContext();
            }

            return new TenantContext
            {
                TenantId = context.User.FindFirstValue("tenant_id") ?? context.Request.Headers["x-tenant-id"].ToString(),
                UserId = context.User.FindFirstValue("user_id") ?? string.Empty,
                Role = context.User.FindFirstValue(ClaimTypes.Role) ?? context.User.FindFirstValue("role") ?? string.Empty,
                CorrelationId = context.TraceIdentifier
            };
        }
    }
}
