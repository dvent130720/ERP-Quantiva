using Quantiva.Application.DTOs;

namespace Quantiva.Application.Common.Abstractions;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(UpsertCustomerDto request, CancellationToken cancellationToken = default);
    Task<CustomerDto> UpdateAsync(Guid id, UpsertCustomerDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
