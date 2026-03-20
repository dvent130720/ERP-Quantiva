namespace External.Web.Api.Gateway.Infrastructure.Proxy;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public bool Enabled { get; init; }

    public int PermitLimit { get; init; } = 100;

    public int QueueLimit { get; init; } = 0;

    public int WindowSeconds { get; init; } = 60;
}
