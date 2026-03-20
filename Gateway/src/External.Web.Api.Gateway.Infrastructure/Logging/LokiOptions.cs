namespace External.Web.Api.Gateway.Infrastructure.Logging;

public sealed class LokiOptions
{
    public const string SectionName = "Observability:Loki";

    public string? Url { get; init; }
}
