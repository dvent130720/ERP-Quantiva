namespace SriFacturacion.Infrastructure.Opciones;

public sealed class RabbitMqOptions
{
    public const string Seccion = "RabbitMq";
    public string Host { get; set; } = "rabbitmq";
    public string Usuario { get; set; } = "guest";
    public string Clave { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public int Puerto { get; set; } = 5672;
    public string Exchange { get; set; } = "sri.exchange";
    public string SufijoDeadLetter { get; set; } = ".dlq";
}
