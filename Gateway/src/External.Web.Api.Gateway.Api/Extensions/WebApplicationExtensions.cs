using External.Web.Api.Gateway.Api.Middlewares;
using External.Web.Api.Gateway.Infrastructure;
using External.Web.Api.Gateway.Infrastructure.Proxy;

namespace External.Web.Api.Gateway.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseGatewayApi(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<CorrelationMiddleware>();
        app.UseInfrastructureObservability();
        app.UseForwardedHeaders();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health").AllowAnonymous();
        app.MapGet("/", () => Results.Ok(new { service = "External.Web.Api.Gateway", status = "running" }))
            .AllowAnonymous();
        app.MapGatewayReverseProxy();

        return app;
    }
}
