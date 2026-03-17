using Billing.Service.Api;
using Billing.Service.Application;
using Billing.Service.Domain;
using Billing.Service.Infrastructure;
using Billing.Service.Application.Abstractions;
using Billing.Service.Infrastructure.Security;
using Billing.Service.Infrastructure.Signing;
using Billing.Service.Infrastructure.Tenants;
using BuildingBlocks.Contracts;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BillingDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("BillingDb") ?? "Host=postgres;Database=billing;Username=postgres;Password=postgres";
    opt.UseNpgsql(cs);
});
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ITenantCertificateClient, TenantCertificateClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Tenant"] ?? "http://tenant-service:8080");
});
builder.Services.AddScoped<ICertificateProvider, CertificateProvider>();
builder.Services.AddScoped<IXmlSigner, XmlSigner>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/facturas", async (CreateInvoiceRequest request, BillingDbContext db, IConfiguration config) =>
{
    var claveAcceso = AccessKeyGenerator.Generate(request.Ruc, DateOnly.FromDateTime(DateTime.UtcNow), request.Secuencial);
    if (await db.Comprobantes.AnyAsync(x => x.TenantId == request.TenantId && x.ClaveAcceso == claveAcceso))
    {
        return Results.Conflict(new { message = "Comprobante duplicado", claveAcceso });
    }

    var entity = new Comprobante
    {
        Id = Guid.NewGuid(),
        TenantId = request.TenantId,
        ClaveAcceso = claveAcceso,
        Xml = $"<factura><total>{request.Total}</total></factura>",
        Estado = "CREADA"
    };

    db.Comprobantes.Add(entity);
    await db.SaveChangesAsync();

    var ev = new InvoiceCreated(entity.Id, request.TenantId, entity.ClaveAcceso, Guid.NewGuid(), DateTimeOffset.UtcNow);
    PublishEvent(config, "FacturaCreada", ev);

    return Results.Accepted($"/facturas/{entity.Id}", new { entity.Id, entity.ClaveAcceso, entity.Estado });
});

app.MapGet("/facturas/{id:guid}", async (Guid id, BillingDbContext db) =>
    await db.Comprobantes.FindAsync(id) is { } row ? Results.Ok(row) : Results.NotFound());

app.MapGet("/facturas/{clave}/estado", async (string clave, BillingDbContext db) =>
    await db.Comprobantes.FirstOrDefaultAsync(x => x.ClaveAcceso == clave) is { } row ? Results.Ok(new { row.ClaveAcceso, row.Estado, row.Errores }) : Results.NotFound());

app.MapPost("/internal/facturas/{id:guid}/estado", async (Guid id, InvoiceStatusUpdate req, BillingDbContext db) =>
{
    var row = await db.Comprobantes.FindAsync(id);
    if (row is null) return Results.NotFound();

    row.Estado = req.Estado;
    row.XmlFirmado = req.XmlFirmado ?? row.XmlFirmado;
    row.XmlAutorizado = req.XmlAutorizado ?? row.XmlAutorizado;
    row.Errores = req.Error ?? row.Errores;
    row.UpdatedAt = DateTimeOffset.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();

static void PublishEvent(IConfiguration config, string routingKey, object body)
{
    var factory = new ConnectionFactory { HostName = config["RabbitMQ:Host"] ?? "rabbitmq" };
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();
    channel.ExchangeDeclare("billing.events", ExchangeType.Topic, durable: true);
    var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body));
    channel.BasicPublish("billing.events", routingKey, null, payload);
}
