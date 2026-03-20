namespace SriFacturacion.Application.DTOs;

public sealed record CrearFacturaResponse(Guid FacturaId, string ClaveAcceso, string Estado, string Mensaje);
