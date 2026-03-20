using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using SriFacturacion.Application.Abstracciones.Bus;
using SriFacturacion.Application.Abstracciones.Catalogos;
using SriFacturacion.Application.Abstracciones.Certificados;
using SriFacturacion.Application.Abstracciones.Correo;
using SriFacturacion.Application.Abstracciones.Documentos;
using SriFacturacion.Application.Abstracciones.Infraestructura;
using SriFacturacion.Application.Abstracciones.Persistencia;
using SriFacturacion.Application.Abstracciones.Sri;
using SriFacturacion.Infrastructure.Cifrado;
using SriFacturacion.Infrastructure.Mensajeria;
using SriFacturacion.Infrastructure.Opciones;
using SriFacturacion.Infrastructure.Persistencia;
using SriFacturacion.Infrastructure.Repositorios;
using SriFacturacion.Infrastructure.Servicios.Archivos;
using SriFacturacion.Infrastructure.Servicios.Catalogos;
using SriFacturacion.Infrastructure.Servicios.Certificados;
using SriFacturacion.Infrastructure.Servicios.Correo;
using SriFacturacion.Infrastructure.Servicios.Documentos;
using SriFacturacion.Infrastructure.Servicios.Migraciones;
using SriFacturacion.Infrastructure.Servicios.Sri;

namespace SriFacturacion.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool registrarMigrador = true)
    {
        services.Configure<PostgresOptions>(configuration.GetSection(PostgresOptions.Seccion));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Seccion));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.Seccion));
        services.Configure<SriSoapOptions>(configuration.GetSection(SriSoapOptions.Seccion));
        services.Configure<SeguridadOptions>(configuration.GetSection(SeguridadOptions.Seccion));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.Seccion));
        services.Configure<ObservabilidadOptions>(configuration.GetSection(ObservabilidadOptions.Seccion));

        services.AddSingleton<IConexionFactory, ConexionFactory>();
        services.AddSingleton<IPublicadorEventos, RabbitMqPublisher>();
        services.AddSingleton<IConsumidorEventos, RabbitMqConsumer>();
        services.AddScoped<IFacturaRepository, FacturasRepository>();
        services.AddScoped<ICertificadoRepository, CertificadosRepository>();
        services.AddScoped<ICatalogoRepository, CatalogosRepository>();
        services.AddScoped<IProveedorCatalogos, ProveedorCatalogos>();
        services.AddSingleton<ICifrador, AesCifrador>();
        services.AddSingleton<IAlmacenCertificados, AlmacenCertificadosLocal>();
        services.AddSingleton<IRepositorioArchivosFactura, RepositorioArchivosFacturaLocal>();
        services.AddScoped<IValidadorCertificado, ValidadorCertificadoPkcs12>();
        services.AddScoped<IGeneradorClaveAcceso, GeneradorClaveAccesoEcuador>();
        services.AddScoped<IGeneradorXmlFactura, GeneradorXmlFacturaEcuador>();
        services.AddScoped<IValidadorXmlSri, ValidadorXmlSri>();
        services.AddScoped<IFirmadorXml, FirmadorXmlPkcs12>();
        services.AddScoped<IGeneradorPdfFactura, GeneradorPdfFacturaQuest>();
        services.AddScoped<IEmailSender, EmailSenderZoho>();
        services.AddHttpClient<IClienteSriSoap, ClienteSriSoap>();

        if (registrarMigrador)
        {
            services.AddHostedService<HostedMigracionInicial>();
        }

        return services;
    }

    public static void ConfigureSerilogEstructurado(IConfiguration configuration)
    {
        var options = configuration.GetSection(ObservabilidadOptions.Seccion).Get<ObservabilidadOptions>() ?? new ObservabilidadOptions();
        Directory.CreateDirectory(Path.GetDirectoryName(options.RutaLogs) ?? "/app/logs");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Servicio", options.NombreServicio)
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .WriteTo.File(new RenderedCompactJsonFormatter(), options.RutaLogs, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14, shared: true)
            .CreateLogger();
    }
}
