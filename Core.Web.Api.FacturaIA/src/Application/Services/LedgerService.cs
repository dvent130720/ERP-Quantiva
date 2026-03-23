using Core.Web.Api.FacturaIA.Application.DTOs;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Domain.Entities;

namespace Core.Web.Api.FacturaIA.Application.Services;

public class LedgerService
{
    private readonly ILedgerRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public LedgerService(ILedgerRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<LedgerEntry> CrearAsync(LedgerEntryCreateDto dto, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var entity = new LedgerEntry
        {
            TenantId = tenantId,
            Fecha = dto.Fecha,
            Tipo = dto.Tipo,
            CuentaDebito = dto.CuentaDebito,
            CuentaCredito = dto.CuentaCredito,
            Monto = dto.Monto,
            Moneda = dto.Moneda,
            VentaId = dto.VentaId
        };

        await _repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public Task<IReadOnlyList<LedgerEntry>> ObtenerRangoAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default)
        => _repo.GetByRangeAsync(inicio, fin, tenantId, cancellationToken);
}
