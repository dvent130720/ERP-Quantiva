namespace Core.Web.Api.FacturaIA.Shared.Models;

public record ApiResponse<T>(bool Success, T Data, string? Message = null);
