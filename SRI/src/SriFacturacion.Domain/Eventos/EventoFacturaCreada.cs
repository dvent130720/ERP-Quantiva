namespace SriFacturacion.Domain.Eventos;

public sealed record EventoFacturaCreada(Guid FacturaId, string CorrelationId, DateTimeOffset Fecha);
