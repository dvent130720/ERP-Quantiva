using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Web.Api.FacturaIA.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<VentasService>();
        services.AddScoped<LedgerService>();
        services.AddScoped<IAiService, AiService>();
        return services;
    }
}
