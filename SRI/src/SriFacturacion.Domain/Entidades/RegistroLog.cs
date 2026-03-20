namespace SriFacturacion.Domain.Entidades;

public sealed class RegistroLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CorrelationId { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? Datos { get; set; }
    public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;
}
