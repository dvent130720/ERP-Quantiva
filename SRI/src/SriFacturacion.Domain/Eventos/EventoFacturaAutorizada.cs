namespace SriFacturacion.Domain.Eventos;

public sealed record EventoFacturaAutorizada(Guid FacturaId, string CorrelationId, string ClaveAcceso, string CorreoCliente, DateTimeOffset FechaAutorizacion);
