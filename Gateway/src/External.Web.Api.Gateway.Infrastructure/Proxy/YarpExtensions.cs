using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace External.Web.Api.Gateway.Infrastructure.Proxy;

public static class YarpExtensions
{
    public static IServiceCollection AddGatewayProxy(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("Yarp"))
            .AddTransforms<UserIdTransformProvider>();

        return services;
    }

    public static IServiceCollection AddGatewayHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }

    public static IServiceCollection AddGatewayRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName))
            .ValidateOnStart();

        services.AddRateLimiter((serviceProvider, options) =>
        {
            var rateLimitingOptions = serviceProvider.GetRequiredService<IOptions<RateLimitingOptions>>().Value;
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            if (!rateLimitingOptions.Enabled)
            {
                return;
            }

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitingOptions.PermitLimit,
                        QueueLimit = rateLimitingOptions.QueueLimit,
                        Window = TimeSpan.FromSeconds(rateLimitingOptions.WindowSeconds),
                        AutoReplenishment = true
                    }));
        });

        return services;
    }

    public static IEndpointRouteBuilder MapGatewayReverseProxy(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapReverseProxy();
        return endpoints;
    }
}
