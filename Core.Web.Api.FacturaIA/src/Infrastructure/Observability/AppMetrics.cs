using Core.Web.Api.FacturaIA.Application.Interfaces;
using Prometheus;

namespace Core.Web.Api.FacturaIA.Infrastructure.Observability;

public class AppMetrics : IAppMetrics
{
    private static readonly Counter AiIntentCounter = Metrics.CreateCounter("facturaia_ai_intents_total", "Conteo de intents IA procesados.", new CounterConfiguration
    {
        LabelNames = new[] { "intent" }
    });

    private static readonly Counter SriSubmissionCounter = Metrics.CreateCounter("facturaia_sri_submissions_total", "Conteo de envíos al SRI.", new CounterConfiguration
    {
        LabelNames = new[] { "status" }
    });

    public void RecordAiIntent(string intent) => AiIntentCounter.WithLabels(intent).Inc();

    public void RecordSriSubmission(string status) => SriSubmissionCounter.WithLabels(status).Inc();
}
