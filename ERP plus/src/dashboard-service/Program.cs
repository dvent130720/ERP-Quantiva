using Microsoft.Extensions.Caching.Distributed;
using MultiTenant;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().WriteTo.Console().WriteTo.Seq(builder.Configuration["Serilog:SeqUrl"] ?? "http://seq:5341").CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddCommonPlatform(builder.Configuration, "dashboard-service");
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddCommonHealthChecks(builder.Configuration);
builder.Services.AddHttpClient("internal").AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200*i)));

var app = builder.Build();
app.UseCommonPlatform();
app.MapHealthChecks("/health");
app.MapGet("/api/dashboard", async (IHttpClientFactory factory, ITenantProvider tenant, IDistributedCache cache) =>
{
    var key = $"dashboard:{tenant.Current.TenantId}";
    var cached = await cache.GetStringAsync(key);
    if (!string.IsNullOrWhiteSpace(cached)) return Results.Content(cached, "application/json");

    var client = factory.CreateClient("internal");
    var invoices = await client.GetStringAsync("http://invoice-service:8080/api/invoice");
    var clients = await client.GetStringAsync("http://client-service:8080/api/client");
    var payload = JsonSerializer.Serialize(new
    {
        kpis = new { totalClients = JsonDocument.Parse(clients).RootElement.GetArrayLength(), totalInvoices = JsonDocument.Parse(invoices).RootElement.GetArrayLength() },
        recentInvoices = JsonDocument.Parse(invoices).RootElement,
        activity = new[] { "InvoiceCreated", "ClientCreated" },
        metrics = new { tenant = tenant.Current.TenantId, generatedAt = DateTime.UtcNow }
    });

    await cache.SetStringAsync(key, payload, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) });
    return Results.Content(payload, "application/json");
}).RequireAuthorization();

app.Run();
