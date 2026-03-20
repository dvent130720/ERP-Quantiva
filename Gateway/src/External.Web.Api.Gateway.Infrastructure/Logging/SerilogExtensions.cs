using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace External.Web.Api.Gateway.Infrastructure.Logging;

public static class SerilogExtensions
{
    public static IHostBuilder AddStructuredLogging(this IHostBuilder host, IConfiguration configuration)
    {
        host.UseSerilog((context, services, loggerConfiguration) =>
        {
            var lokiOptions = configuration.GetSection(LokiOptions.SectionName).Get<LokiOptions>();

            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console();

            if (!string.IsNullOrWhiteSpace(lokiOptions?.Url))
            {
                loggerConfiguration.WriteTo.GrafanaLoki(
                    lokiOptions.Url,
                    labels: new[]
                    {
                        new LokiLabel { Key = "application", Value = "external-web-api-gateway" },
                        new LokiLabel { Key = "environment", Value = context.HostingEnvironment.EnvironmentName }
                    });
            }
        });

        return host;
    }
}
