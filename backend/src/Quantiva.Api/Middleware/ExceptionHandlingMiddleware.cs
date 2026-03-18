using System.Net;
using Quantiva.Application.Common.Exceptions;

namespace Quantiva.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled application exception");
            await WriteProblemAsync(context, exception);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            ValidationAppException => (HttpStatusCode.BadRequest, "Validation error"),
            NotFoundAppException => (HttpStatusCode.NotFound, "Resource not found"),
            ConflictAppException => (HttpStatusCode.Conflict, "Conflict"),
            TenantResolutionException => (HttpStatusCode.BadRequest, "Tenant resolution failed"),
            _ => (HttpStatusCode.InternalServerError, "Internal server error")
        };

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new
        {
            type = $"https://httpstatuses.com/{(int)status}",
            title,
            status = (int)status,
            detail = exception.Message,
            traceId = context.TraceIdentifier
        });
    }
}
