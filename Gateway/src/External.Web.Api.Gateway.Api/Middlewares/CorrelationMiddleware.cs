using External.Web.Api.Gateway.Application.Interfaces;
using External.Web.Api.Gateway.Domain.Models;
using Serilog.Context;

namespace External.Web.Api.Gateway.Api.Middlewares;

public sealed class CorrelationMiddleware(RequestDelegate next)
{
    private const string TraceHeader = "X-Trace-Id";

    public async Task InvokeAsync(HttpContext context, ITraceIdentifierFactory traceIdentifierFactory, ITraceContextAccessor traceContextAccessor)
    {
        var traceId = context.Request.Headers[TraceHeader].FirstOrDefault();
        traceId = string.IsNullOrWhiteSpace(traceId) ? traceIdentifierFactory.Create() : traceId;

        context.TraceIdentifier = traceId;
        context.Response.Headers[TraceHeader] = traceId;
        traceContextAccessor.Current = new TraceContext { TraceId = traceId };

        using (LogContext.PushProperty("TraceId", traceId))
        {
            await next(context);
        }
    }
}
