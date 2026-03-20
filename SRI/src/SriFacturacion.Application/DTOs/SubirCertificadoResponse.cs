namespace SriFacturacion.Application.DTOs;

public sealed record SubirCertificadoResponse(Guid CertificadoId, DateTimeOffset FechaExpiracion, string Thumbprint);
