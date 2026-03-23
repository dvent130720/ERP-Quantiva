using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Domain.Entities;
using Core.Web.Api.FacturaIA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Web.Api.FacturaIA.Infrastructure.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly AppDbContext _context;

    public VentaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> ObtenerTotalAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default)
        => await _context.Ventas
            .Where(x => x.TenantId == tenantId && x.Fecha >= inicio && x.Fecha <= fin)
            .SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0m;

    public async Task<Venta> AddAsync(Venta venta, CancellationToken cancellationToken = default)
    {
        await _context.Ventas.AddAsync(venta, cancellationToken);
        return venta;
    }

    public Task<Venta?> GetByIdAsync(Guid ventaId, Guid tenantId, CancellationToken cancellationToken = default)
        => _context.Ventas.FirstOrDefaultAsync(x => x.Id == ventaId && x.TenantId == tenantId, cancellationToken);
}
