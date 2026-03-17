using BuildingBlocks.Contracts;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<InvoiceWorkflowWorker>();
var app = builder.Build();
app.MapGet("/health", () => Results.Ok("worker-running"));
app.Run();

sealed class InvoiceWorkflowWorker(IConfiguration config, ILogger<InvoiceWorkflowWorker> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() => StartConsumer(stoppingToken), stoppingToken);
        return Task.CompletedTask;
    }

    private void StartConsumer(CancellationToken ct)
    {
        var factory = new ConnectionFactory { HostName = config["RabbitMQ:Host"] ?? "rabbitmq" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("billing.events", ExchangeType.Topic, durable: true);
        channel.QueueDeclare("worker.billing", durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind("worker.billing", "billing.events", "FacturaCreada");
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) =>
        {
            var payload = Encoding.UTF8.GetString(ea.Body.ToArray());
            var created = JsonSerializer.Deserialize<InvoiceCreated>(payload);
            if (created is null) return;

            var retry = Policy.Handle<Exception>().WaitAndRetry(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            retry.Execute(() => logger.LogInformation("Procesando factura {InvoiceId} tenant {TenantId}", created.InvoiceId, created.TenantId));
        };

        channel.BasicConsume("worker.billing", autoAck: true, consumer);
        while (!ct.IsCancellationRequested) Thread.Sleep(500);
    }
}
