namespace Quantiva.Application.DTOs;

public sealed record ProductDto(Guid Id, string Sku, string Name, string? Description, decimal Price, bool IsActive, DateTime UpdatedAtUtc);
public sealed record UpsertProductDto(string Sku, string Name, string? Description, decimal Price, bool IsActive);
