namespace SriFacturacion.Application.DTOs;

public sealed record FacturaEstadoResponse(Guid FacturaId, string Estado, string Mensaje, string ClaveAcceso, DateTimeOffset? FechaAutorizacion);
