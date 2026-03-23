using Core.Web.Api.FacturaIA.Application.DTOs;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Domain.Entities;

namespace Core.Web.Api.FacturaIA.Application.Services;

public class VentasService
{
    private readonly IVentaRepository _repo;
    private readonly ILedgerRepository _ledgerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VentasService(IVentaRepository repo, ILedgerRepository ledgerRepository, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _ledgerRepository = ledgerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> ObtenerTotalAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default)
        => await _repo.ObtenerTotalAsync(inicio, fin, tenantId, cancellationToken);

    public async Task<Venta> CrearVentaAsync(VentaCreateDto dto, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var venta = new Venta
        {
            TenantId = tenantId,
            Fecha = dto.Fecha,
            Total = dto.Total,
            Estado = dto.Estado,
            NumeroComprobante = dto.NumeroComprobante,
            Moneda = dto.Moneda
        };

        await _repo.AddAsync(venta, cancellationToken);
        await _ledgerRepository.AddAsync(new LedgerEntry
        {
            TenantId = tenantId,
            Fecha = dto.Fecha,
            Tipo = "Venta",
            CuentaDebito = "110101",
            CuentaCredito = "410101",
            Monto = dto.Total,
            Moneda = dto.Moneda,
            Venta = venta
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return venta;
    }
}
