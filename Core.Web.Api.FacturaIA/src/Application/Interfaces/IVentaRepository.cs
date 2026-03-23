using Core.Web.Api.FacturaIA.Domain.Entities;

namespace Core.Web.Api.FacturaIA.Application.Interfaces;

public interface IVentaRepository
{
    Task<decimal> ObtenerTotalAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default);
    Task<Venta> AddAsync(Venta venta, CancellationToken cancellationToken = default);
    Task<Venta?> GetByIdAsync(Guid ventaId, Guid tenantId, CancellationToken cancellationToken = default);
}
