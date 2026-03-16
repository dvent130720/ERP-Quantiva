using Serilog.Context;

namespace MultiTenant;

public sealed class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        var tenant = tenantProvider.Current;
        if (context.User.Identity?.IsAuthenticated == true && string.IsNullOrWhiteSpace(tenant.TenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "tenant_id claim is required" });
            return;
        }

        using (LogContext.PushProperty("tenant_id", tenant.TenantId))
        using (LogContext.PushProperty("correlation_id", tenant.CorrelationId))
        {
            context.Response.Headers["x-correlation-id"] = tenant.CorrelationId;
            await next(context);
        }
    }
}
