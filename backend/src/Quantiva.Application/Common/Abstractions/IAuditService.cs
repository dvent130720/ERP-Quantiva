using Quantiva.Application.DTOs;

namespace Quantiva.Application.Common.Abstractions;

public interface IAuditService
{
    Task<IReadOnlyList<AuditTrailDto>> GetLatestAsync(int take = 100, CancellationToken cancellationToken = default);
}
