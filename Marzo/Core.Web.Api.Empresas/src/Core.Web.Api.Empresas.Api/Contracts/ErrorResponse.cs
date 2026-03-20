namespace Core.Web.Api.Empresas.Api.Contracts;

public sealed record ErrorResponse(string Code, string Message, string TraceId);
