namespace External.Web.Api.Gateway.Domain.Common;

public sealed class GatewayError
{
    public required string TraceId { get; init; }

    public required int StatusCode { get; init; }

    public required string Title { get; init; }

    public required string Detail { get; init; }
}
