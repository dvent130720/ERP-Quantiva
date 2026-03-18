using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Infrastructure;
using Quantiva.Infrastructure.Persistence;
using Quantiva.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Quantiva Multi-tenant API",
        Version = "v1",
        Description = "Backend .NET 8 para operaciones multi-tenant, auditoría y administración de datos."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    var tenantContext = context.RequestServices.GetRequiredService<ITenantContext>();
    tenantContext.Clear();

    var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
    var host = context.Request.Host.Host?.ToLowerInvariant();
    var headerSlug = context.Request.Headers["X-Tenant-Slug"].FirstOrDefault()?.Trim().ToLowerInvariant();

    var tenant = headerSlug is not null
        ? await dbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.Slug == headerSlug, context.RequestAborted)
        : null;

    if (tenant is null && host is not null)
    {
        var tenants = await dbContext.Tenants.AsNoTracking().ToListAsync(context.RequestAborted);
        tenant = tenants.FirstOrDefault(x =>
            x.PrimaryDomain == host ||
            host == $"{x.Slug}.quantiva-solutions.com" ||
            host == $"{x.Slug}.localhost");
    }

    if (tenant is not null)
    {
        tenantContext.Set(tenant.Id, tenant.Slug, tenant.Name);
    }

    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.Run();
