namespace Core.Web.Api.Empresas.Infrastructure.Observability;

public sealed class LokiOptions
{
    public const string SectionName = "Loki";
    public string Url { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
