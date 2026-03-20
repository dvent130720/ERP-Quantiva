namespace SriFacturacion.Application.DTOs;

public sealed record EventoFacturaAutorizadaDto(Guid FacturaId, string CorrelationId, string ClaveAcceso, string CorreoCliente, DateTimeOffset FechaAutorizacion);
