using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SriFacturacion.Application.Abstracciones.Bus;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Mensajeria;

public sealed class RabbitMqPublisher : IPublicadorEventos, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Puerto,
            UserName = _options.Usuario,
            Password = _options.Clave,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Direct, durable: true, autoDelete: false);
    }

    public Task PublicarAsync<T>(string cola, T mensaje, CancellationToken cancellationToken) where T : class
    {
        DeclararColaConDeadLetter(cola);
        var payload = JsonSerializer.SerializeToUtf8Bytes(mensaje);
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        _channel.BasicPublish(_options.Exchange, cola, mandatory: false, basicProperties: properties, body: payload);
        _logger.LogInformation("Evento publicado en la cola {Cola}", cola);
        return Task.CompletedTask;
    }

    private void DeclararColaConDeadLetter(string cola)
    {
        _channel.QueueDeclare($"{cola}{_options.SufijoDeadLetter}", durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind($"{cola}{_options.SufijoDeadLetter}", _options.Exchange, $"{cola}{_options.SufijoDeadLetter}");

        var argumentos = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = _options.Exchange,
            ["x-dead-letter-routing-key"] = $"{cola}{_options.SufijoDeadLetter}"
        };

        _channel.QueueDeclare(cola, durable: true, exclusive: false, autoDelete: false, arguments: argumentos);
        _channel.QueueBind(cola, _options.Exchange, cola);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
