using System.Security.Claims;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace External.Web.Api.Gateway.Infrastructure.Proxy;

public sealed class UserIdTransformProvider : ITransformProvider
{
    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(transformContext =>
        {
            var userId = transformContext.HttpContext.User.FindFirstValue("sub");

            if (!string.IsNullOrWhiteSpace(userId))
            {
                transformContext.ProxyRequest.Headers.Remove("X-User-Id");
                transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId);
            }

            var traceId = transformContext.HttpContext.Response.Headers["X-Trace-Id"].ToString();
            if (!string.IsNullOrWhiteSpace(traceId))
            {
                transformContext.ProxyRequest.Headers.Remove("X-Trace-Id");
                transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-Trace-Id", traceId);
            }

            return ValueTask.CompletedTask;
        });
    }
}
