using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SriFacturacion.Application.Abstracciones.Bus;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Mensajeria;

public sealed class RabbitMqConsumer : IConsumidorEventos, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqConsumer(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConsumer> logger)
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
        _channel.BasicQos(0, 1, false);
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Direct, durable: true, autoDelete: false);
    }

    public Task IniciarConsumoAsync<T>(string cola, Func<T, string, Task<bool>> manejador, CancellationToken cancellationToken) where T : class
    {
        DeclararColaConDeadLetter(cola);
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            var correlationId = args.BasicProperties?.CorrelationId ?? Guid.NewGuid().ToString("N");
            try
            {
                var mensaje = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(args.Body.ToArray()));
                if (mensaje is null)
                {
                    _channel.BasicNack(args.DeliveryTag, false, false);
                    return;
                }

                var procesado = await manejador(mensaje, correlationId);
                if (procesado)
                {
                    _channel.BasicAck(args.DeliveryTag, false);
                }
                else
                {
                    _channel.BasicNack(args.DeliveryTag, false, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo consumiendo cola {Cola}", cola);
                _channel.BasicNack(args.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(cola, autoAck: false, consumer);
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
