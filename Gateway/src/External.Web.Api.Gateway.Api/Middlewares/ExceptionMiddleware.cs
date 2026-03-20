using System.Net;
using System.Text.Json;
using External.Web.Api.Gateway.Application.Interfaces;
using External.Web.Api.Gateway.Domain.Common;

namespace External.Web.Api.Gateway.Api.Middlewares;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, ITraceContextAccessor traceContextAccessor)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new GatewayError
            {
                TraceId = traceContextAccessor.Current?.TraceId ?? context.TraceIdentifier,
                StatusCode = context.Response.StatusCode,
                Title = "GatewayError",
                Detail = "An unexpected error occurred while processing the request."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
