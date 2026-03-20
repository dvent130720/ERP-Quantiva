using External.Web.Api.Gateway.Application.Interfaces;

namespace External.Web.Api.Gateway.Application.Services;

public sealed class TraceIdentifierFactory : ITraceIdentifierFactory
{
    public string Create()
    {
        return Guid.NewGuid().ToString("N");
    }
}
