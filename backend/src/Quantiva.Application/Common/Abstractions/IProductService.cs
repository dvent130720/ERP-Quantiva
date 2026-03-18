using Quantiva.Application.DTOs;

namespace Quantiva.Application.Common.Abstractions;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(UpsertProductDto request, CancellationToken cancellationToken = default);
}
