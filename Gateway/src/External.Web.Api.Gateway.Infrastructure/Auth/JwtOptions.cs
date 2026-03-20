namespace External.Web.Api.Gateway.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    public required string Authority { get; init; }

    public required string Audience { get; init; }

    public bool RequireHttpsMetadata { get; init; } = true;
}
