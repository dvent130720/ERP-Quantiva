using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Domain.Entities;
using Core.Web.Api.FacturaIA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Web.Api.FacturaIA.Infrastructure.Repositories;

public class LedgerRepository : ILedgerRepository
{
    private readonly AppDbContext _context;

    public LedgerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LedgerEntry> AddAsync(LedgerEntry entry, CancellationToken cancellationToken = default)
    {
        await _context.Ledger.AddAsync(entry, cancellationToken);
        return entry;
    }

    public async Task<IReadOnlyList<LedgerEntry>> GetByRangeAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default)
        => await _context.Ledger
            .Where(x => x.TenantId == tenantId && x.Fecha >= inicio && x.Fecha <= fin)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync(cancellationToken);
}
