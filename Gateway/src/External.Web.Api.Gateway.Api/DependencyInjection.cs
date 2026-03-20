using External.Web.Api.Gateway.Application.Interfaces;
using External.Web.Api.Gateway.Application.Services;
using External.Web.Api.Gateway.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace External.Web.Api.Gateway.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITraceIdentifierFactory, TraceIdentifierFactory>();
        services.AddInfrastructure(configuration);
        services.AddHttpContextAccessor();
        services.AddProblemDetails();
        return services;
    }
}
