using Core.Web.Api.FacturaIA.Domain.Entities;

namespace Core.Web.Api.FacturaIA.Application.Interfaces;

public interface ILedgerRepository
{
    Task<LedgerEntry> AddAsync(LedgerEntry entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LedgerEntry>> GetByRangeAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default);
}
