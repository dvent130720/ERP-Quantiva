using Microsoft.EntityFrameworkCore;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;
using Quantiva.Infrastructure.Persistence;

namespace Quantiva.Infrastructure.Services;

public sealed class AuditService(AppDbContext dbContext, ITenantContext tenantContext) : IAuditService
{
    public async Task<IReadOnlyList<AuditTrailDto>> GetLatestAsync(int take = 100, CancellationToken cancellationToken = default)
    {
        var query = dbContext.AuditTrails.AsNoTracking();

        if (tenantContext.TenantId.HasValue)
        {
            query = query.Where(x => x.TenantId == tenantContext.TenantId);
        }

        return await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(Math.Clamp(take, 1, 200))
            .Select(x => new AuditTrailDto(x.Id, x.TenantId, x.EntityName, x.EntityId, x.Action, x.UserEmail, x.SourceIp, x.OccurredAtUtc, x.ChangesJson))
            .ToListAsync(cancellationToken);
    }
}
