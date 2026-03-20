using Prometheus;
using Serilog;
using SriFacturacion.Application.Extensions;
using SriFacturacion.Infrastructure.Extensions;
using SriFacturacion.Infrastructure.Opciones;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables();

SriFacturacion.Infrastructure.Extensions.DependencyInjection.ConfigureSerilogEstructurado(builder.Configuration);
builder.Host.UseSerilog();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<ProcesadorFacturasBackgroundService>();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration[$"{PostgresOptions.Seccion}:CadenaConexion"]!, tags: new[] { "ready" })
    .AddRabbitMQ(sp => $"amqp://{builder.Configuration[$"{RabbitMqOptions.Seccion}:Usuario"]}:{builder.Configuration[$"{RabbitMqOptions.Seccion}:Clave"]}@{builder.Configuration[$"{RabbitMqOptions.Seccion}:Host"]}:{builder.Configuration[$"{RabbitMqOptions.Seccion}:Puerto"]}{builder.Configuration[$"{RabbitMqOptions.Seccion}:VirtualHost"]}", tags: new[] { "ready" })
    .AddRedis(builder.Configuration[$"{RedisOptions.Seccion}:CadenaConexion"]!, tags: new[] { "ready" });

var app = builder.Build();
app.UseHttpMetrics();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.MapMetrics("/metrics");
app.Run();
