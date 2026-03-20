using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace Core.Web.Api.Empresas.Infrastructure.Observability;

public static class LoggingExtensions
{
    public static ConfigureHostBuilder AddLokiSerilog(this ConfigureHostBuilder host, IConfiguration configuration)
    {
        host.UseSerilog((context, services, loggerConfiguration) =>
        {
            var lokiOptions = configuration.GetSection(LokiOptions.SectionName).Get<LokiOptions>() ?? new LokiOptions();

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console();

            if (!string.IsNullOrWhiteSpace(lokiOptions.Url))
            {
                loggerConfiguration.WriteTo.GrafanaLoki(
                    lokiOptions.Url,
                    labels: new[]
                    {
                        new LokiLabel { Key = "app", Value = "core-web-api-empresas" },
                        new LokiLabel { Key = "environment", Value = context.HostingEnvironment.EnvironmentName }
                    },
                    credentials: string.IsNullOrWhiteSpace(lokiOptions.Username)
                        ? null
                        : new LokiCredentials
                        {
                            Login = lokiOptions.Username,
                            Password = lokiOptions.Password
                        });
            }
        });

        return host;
    }
}
