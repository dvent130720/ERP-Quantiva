using System.Text.Json;
using Core.Web.Api.Empresas.Application.DTOs;
using Core.Web.Api.Empresas.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;

namespace Core.Web.Api.Empresas.Api.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapEmpresasEndpoints(this IEndpointRouteBuilder app)
    {
        var companies = app.MapGroup("/api/companies")
            .WithTags("Companies")
            .RequireAuthorization();

        companies.MapGet(string.Empty, async (CompanyService service, CancellationToken cancellationToken) =>
        {
            var result = await service.GetAllAsync(cancellationToken);
            return Results.Ok(result);
        });

        companies.MapGet("/{id:guid}", async (Guid id, CompanyService service, CancellationToken cancellationToken) =>
        {
            var result = await service.GetByIdAsync(id, cancellationToken);
            return Results.Ok(result);
        });

        companies.MapPost(string.Empty, async (CompanyRequest request, CompanyService service, CancellationToken cancellationToken) =>
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/api/companies/{result.Id}", result);
        });

        companies.MapPut("/{id:guid}", async (Guid id, CompanyRequest request, CompanyService service, CancellationToken cancellationToken) =>
        {
            var result = await service.UpdateAsync(id, request, cancellationToken);
            return Results.Ok(result);
        });

        companies.MapDelete("/{id:guid}", async (Guid id, CompanyService service, CancellationToken cancellationToken) =>
        {
            await service.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        var auth = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        auth.MapPost("/login", async (
            LoginRequest request,
            AuthService authService,
            Counter loginCounter,
            CancellationToken cancellationToken) =>
        {
            var result = await authService.LoginAsync(request, cancellationToken);
            loginCounter.WithLabels(result.Role).Inc();
            return Results.Ok(result);
        });

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var payload = new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    checks = report.Entries.Select(entry => new
                    {
                        dependency = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        duration = entry.Value.Duration.TotalMilliseconds
                    })
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        })
        .AllowAnonymous()
        .WithTags("Observability");

        app.MapMetrics("/metrics").AllowAnonymous();

        return app;
    }
}
