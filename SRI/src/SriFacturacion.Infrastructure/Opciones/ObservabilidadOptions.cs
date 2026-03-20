namespace SriFacturacion.Infrastructure.Opciones;

public sealed class ObservabilidadOptions
{
    public const string Seccion = "Observabilidad";
    public string RutaLogs { get; set; } = "/app/logs/log-.ndjson";
    public string NombreServicio { get; set; } = "sri-facturacion";
}
