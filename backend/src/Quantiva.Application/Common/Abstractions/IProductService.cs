using Quantiva.Application.DTOs;

namespace Quantiva.Application.Common.Abstractions;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(UpsertProductDto request, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(Guid id, UpsertProductDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
