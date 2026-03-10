// src/ERPSystem.API/Program.cs
using Carter;
using ERPSystem.API.Extensions;
using ERPSystem.API.Middleware;
using ERPSystem.API.Services;
using ERPSystem.Infrastructure.Persistence;
using ERPSystem.Infrastructure.Services.Identity;
using ERPSystem.Infrastructure.Services.SRI;
using ERPSystem.Infrastructure.Caching;
using ERPSystem.Infrastructure.ExternalServices;
using ERPSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURAR SERVICIOS USANDO EXTENSIONES
// ============================================

// Base de datos
builder.Services.AddDatabaseServices(builder.Configuration);

// Autenticación y Autorización JWT
builder.Services.AddAuthenticationServices(builder.Configuration);

// Requerir autenticación por defecto en TODOS los endpoints
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());
// CORS
builder.Services.AddCorsConfiguration(builder.Configuration);

// Swagger/OpenAPI
builder.Services.AddSwaggerConfiguration();

// Carter (Minimal APIs)
builder.Services.AddCarter();

// ============================================
// REGISTRAR SERVICIOS DE LA APLICACIÓN
// ============================================

// Servicios de negocio
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISriInvoiceService, SriInvoiceService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Cola y worker para PDF + WhatsApp
builder.Services.AddSingleton<IInvoiceDispatchQueue, InvoiceDispatchQueue>();
builder.Services.AddHostedService<InvoicePostProcessingWorker>();

// Repositorios genéricos
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Cache (opcional - descomentar cuando tengas Redis configurado)
// builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
// {
//     var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379");
//     return ConnectionMultiplexer.Connect(configuration);
// });
// builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Logging mejorado (opcional)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ============================================
// CONSTRUIR LA APLICACIÓN
// ============================================

var app = builder.Build();

// ============================================
// APLICAR MIGRACIONES AUTOMÁTICAMENTE
// ============================================

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        
        var appliedMigrations = await db.Database.GetAppliedMigrationsAsync();
        Console.WriteLine($"✅ Migraciones aplicadas: {appliedMigrations.Count()}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error aplicando migraciones: {ex.Message}");
        Console.WriteLine($"   Detalle: {ex.InnerException?.Message}");
    }
}

// ============================================
// CONFIGURAR MIDDLEWARE PIPELINE
// ============================================

// 1. Manejo global de errores (debe ir primero)
app.UseExceptionHandling();

// 2. Request logging para auditoría
app.UseRequestLogging();

// 3. CORS (antes de autenticación)
app.UseCors("AllowAngular");

// 4. Swagger (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP Facturación API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
        c.DocumentTitle = "ERP API - Documentación";
    });
}

// 5. HTTPS Redirection (solo en producción)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

// 6. Autenticación JWT
app.UseAuthentication();

// 7. Autorización
app.UseAuthorization();

// ============================================
// MAPEAR ENDPOINTS
// ============================================

// Carter Endpoints (automáticamente descubre todos los ICarterModule)
app.MapCarter();

// Health Check con información adicional
app.MapGet("/health", async (ApplicationDbContext db) =>
{
    try
    {
        // Verificar conexión a la base de datos
        var canConnect = await db.Database.CanConnectAsync();
        
        return Results.Ok(new
        {
            status = canConnect ? "healthy" : "unhealthy",
            timestamp = DateTime.UtcNow,
            environment = app.Environment.EnvironmentName,
            version = "1.0.0",
            database = new
            {
                connected = canConnect,
                provider = "PostgreSQL"
            }
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new
        {
            status = "unhealthy",
            timestamp = DateTime.UtcNow,
            environment = app.Environment.EnvironmentName,
            error = ex.Message
        });
    }
})
.WithName("HealthCheck")
.WithTags("System")
.AllowAnonymous();

// Info del sistema (sin autenticación)
app.MapGet("/info", () => Results.Ok(new
{
    application = "ERP Sistema de Facturación Electrónica",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow,
    endpoints = new
    {
        swagger = app.Environment.IsDevelopment() ? "/" : null,
        health = "/health",
        auth = "/api/auth",
        invoices = "/api/invoices",
        customers = "/api/customers",
        products = "/api/products",
        tenants = "/api/tenants"
    }
}))
.WithName("SystemInfo")
.WithTags("System")
.AllowAnonymous();

// ============================================
// INICIAR LA APLICACIÓN
// ============================================

var port = builder.Configuration["PORT"] ?? "5012";
var urls = $"http://localhost:{port}";

Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine("🚀 ERP SISTEMA DE FACTURACIÓN ELECTRÓNICA");
Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine($"📍 Ambiente: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔗 URL: {urls}");
Console.WriteLine($"📚 Swagger: {urls}");
Console.WriteLine($"💚 Health Check: {urls}/health");
Console.WriteLine($"ℹ️  Info: {urls}/info");
Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine("✅ API lista para recibir peticiones");
Console.WriteLine("⚡ Presiona Ctrl+C para detener");
Console.WriteLine("════════════════════════════════════════════════════════");

app.Run(urls);