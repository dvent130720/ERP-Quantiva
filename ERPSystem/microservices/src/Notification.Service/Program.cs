using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<NotificationWorker>();
var app = builder.Build();
app.MapGet("/health", () => Results.Ok("notification-running"));
app.Run();

sealed class NotificationWorker(IConfiguration config, ILogger<NotificationWorker> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() => Start(stoppingToken), stoppingToken);
        return Task.CompletedTask;
    }

    private void Start(CancellationToken ct)
    {
        var factory = new ConnectionFactory { HostName = config["RabbitMQ:Host"] ?? "rabbitmq" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("billing.events", ExchangeType.Topic, true);
        channel.QueueDeclare("notifications", true, false, false);
        channel.QueueBind("notifications", "billing.events", "FacturaAutorizada");
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) => logger.LogInformation("Evento para email/webhook: {Body}", Encoding.UTF8.GetString(ea.Body.ToArray()));
        channel.BasicConsume("notifications", true, consumer);
        while (!ct.IsCancellationRequested) Thread.Sleep(500);
    }
}
