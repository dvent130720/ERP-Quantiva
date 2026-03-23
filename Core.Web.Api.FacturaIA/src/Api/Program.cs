using System.Text.Json;
using Core.Web.Api.FacturaIA.Api.Middleware;
using Core.Web.Api.FacturaIA.Application.Extensions;
using Core.Web.Api.FacturaIA.Infrastructure.Configuration;
using Core.Web.Api.FacturaIA.Infrastructure.Extensions;
using FluentValidation.AspNetCore;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = false;
});

builder.Host.UseSerilog((ctx, lc) =>
{
    var observability = ctx.Configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>() ?? new ObservabilityOptions();
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext()
      .Enrich.WithEnvironmentName()
      .Enrich.WithThreadId()
      .WriteTo.Console()
      .WriteTo.GrafanaLoki(observability.LokiUrl, labels: new[]
      {
          new Serilog.Sinks.Grafana.Loki.LokiLabel { Key = "service", Value = observability.ServiceName },
          new Serilog.Sinks.Grafana.Loki.LokiLabel { Key = "environment", Value = ctx.HostingEnvironment.EnvironmentName }
      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditMiddleware>();
app.UseHttpMetrics();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapMetrics();

app.Run();

public partial class Program;
