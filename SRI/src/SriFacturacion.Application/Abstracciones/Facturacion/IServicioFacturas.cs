using SriFacturacion.Application.DTOs;

namespace SriFacturacion.Application.Abstracciones.Facturacion;

public interface IServicioFacturas
{
    Task<CrearFacturaResponse> CrearAsync(CrearFacturaRequest request, string correlationId, CancellationToken cancellationToken);
    Task<FacturaEstadoResponse?> ObtenerEstadoAsync(Guid facturaId, CancellationToken cancellationToken);
    Task ProcesarAsync(Guid facturaId, string correlationId, CancellationToken cancellationToken);
    Task EnviarCorreoAsync(EventoFacturaAutorizadaDto evento, CancellationToken cancellationToken);
}
