using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Abstracciones.Persistencia;

public interface IFacturaRepository
{
    Task CrearAsync(Factura factura, CancellationToken cancellationToken);
    Task<Factura?> ObtenerPorIdAsync(Guid facturaId, CancellationToken cancellationToken);
    Task ActualizarAsync(Factura factura, CancellationToken cancellationToken);
}
