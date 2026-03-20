using System.Net;
using Core.Web.Api.Empresas.Api.Contracts;
using Core.Web.Api.Empresas.Domain.Exceptions;

namespace Core.Web.Api.Empresas.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var (statusCode, code, message) = exception switch
            {
                DomainValidationException => (HttpStatusCode.BadRequest, "validation_error", exception.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, "not_found", exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "unauthorized", exception.Message),
                _ => (HttpStatusCode.InternalServerError, "server_error", "Ocurrió un error inesperado.")
            };

            _logger.LogError(exception, "Error procesando {Method} {Path} con traceId {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ErrorResponse(code, message, context.TraceIdentifier));
        }
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionMiddleware>();
}
