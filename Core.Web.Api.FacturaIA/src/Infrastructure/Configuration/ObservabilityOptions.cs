namespace Core.Web.Api.FacturaIA.Infrastructure.Configuration;

public class ObservabilityOptions
{
    public const string SectionName = "Observability";
    public string ServiceName { get; set; } = "facturaia-api";
    public string LokiUrl { get; set; } = "http://loki:3100";
}
