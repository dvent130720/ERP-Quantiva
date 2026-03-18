using Quantiva.Application.DTOs;

namespace Quantiva.Application.Common.Abstractions;

public interface ITenantAdminService
{
    Task<IReadOnlyList<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantDto> CreateAsync(CreateTenantDto request, CancellationToken cancellationToken = default);
}
