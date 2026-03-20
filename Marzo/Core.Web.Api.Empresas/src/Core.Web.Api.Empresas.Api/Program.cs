using Serilog;
using Core.Web.Api.Empresas.Api.Extensions;
using Core.Web.Api.Empresas.Api.Middleware;
using Core.Web.Api.Empresas.Infrastructure;
using Core.Web.Api.Empresas.Infrastructure.Observability;
using Core.Web.Api.Empresas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLokiSerilog(builder.Configuration);

builder.Services.AddEmpresasInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync(CancellationToken.None);
}

app.UseSerilogRequestLogging();
app.UseGlobalExceptionMiddleware();
app.UseRouting();
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapEmpresasEndpoints();

app.Run();
