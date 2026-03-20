using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Prometheus;
using Serilog;
using SriFacturacion.Api.Configuracion;
using SriFacturacion.Api.Extensions;
using SriFacturacion.Application.Extensions;
using SriFacturacion.Infrastructure.Extensions;
using SriFacturacion.Infrastructure.Opciones;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

SriFacturacion.Infrastructure.Extensions.DependencyInjection.ConfigureSerilogEstructurado(builder.Configuration);
builder.Host.UseSerilog();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration[$"{PostgresOptions.Seccion}:CadenaConexion"]!, tags: new[] { "ready" })
    .AddRabbitMQ(sp => $"amqp://{builder.Configuration[$"{RabbitMqOptions.Seccion}:Usuario"]}:{builder.Configuration[$"{RabbitMqOptions.Seccion}:Clave"]}@{builder.Configuration[$"{RabbitMqOptions.Seccion}:Host"]}:{builder.Configuration[$"{RabbitMqOptions.Seccion}:Puerto"]}{builder.Configuration[$"{RabbitMqOptions.Seccion}:VirtualHost"]}", tags: new[] { "ready" })
    .AddRedis(builder.Configuration[$"{RedisOptions.Seccion}:CadenaConexion"]!, tags: new[] { "ready" });

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseHttpMetrics();
app.MapControllers();
app.MapMetrics("/metrics");
app.MapHealthChecksCustom();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
