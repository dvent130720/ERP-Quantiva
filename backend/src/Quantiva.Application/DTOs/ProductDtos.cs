using System.ComponentModel.DataAnnotations;

namespace Quantiva.Application.DTOs;

public sealed record ProductDto(Guid Id, string Sku, string Name, string? Description, decimal Price, bool IsActive, DateTime UpdatedAtUtc);

public sealed record UpsertProductDto(
    [property: Required, MaxLength(60)] string Sku,
    [property: Required, MaxLength(140)] string Name,
    [property: MaxLength(600)] string? Description,
    [property: Range(typeof(decimal), "0.01", "9999999999999999")] decimal Price,
    bool IsActive);
