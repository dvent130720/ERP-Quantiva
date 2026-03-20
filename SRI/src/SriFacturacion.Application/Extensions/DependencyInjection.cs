using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SriFacturacion.Application.Abstracciones.Facturacion;
using SriFacturacion.Application.Certificados;
using SriFacturacion.Application.DTOs;
using SriFacturacion.Application.Facturas;

namespace SriFacturacion.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IServicioFacturas, ServicioFacturas>();
        services.AddScoped<IServicioCertificados, ServicioCertificados>();
        services.AddScoped<IValidator<CrearFacturaRequest>, CrearFacturaRequestValidator>();
        services.AddScoped<IValidator<SubirCertificadoRequest>, SubirCertificadoRequestValidator>();
        return services;
    }
}
