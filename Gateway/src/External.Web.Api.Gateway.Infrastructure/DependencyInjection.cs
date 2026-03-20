using External.Web.Api.Gateway.Application.Interfaces;
using External.Web.Api.Gateway.Infrastructure.Auth;
using External.Web.Api.Gateway.Infrastructure.Logging;
using External.Web.Api.Gateway.Infrastructure.Proxy;
using External.Web.Api.Gateway.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace External.Web.Api.Gateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITraceContextAccessor, TraceContextAccessor>();
        services.AddJwtAuthentication(configuration);
        services.AddGatewayProxy(configuration);
        services.AddGatewayHealthChecks();
        services.AddGatewayRateLimiting(configuration);
        return services;
    }

    public static IHostBuilder AddInfrastructureLogging(this IHostBuilder host, IConfiguration configuration)
    {
        host.AddStructuredLogging(configuration);
        return host;
    }

    public static IApplicationBuilder UseInfrastructureObservability(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
