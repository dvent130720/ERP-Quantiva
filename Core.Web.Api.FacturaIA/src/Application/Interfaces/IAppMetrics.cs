namespace Core.Web.Api.FacturaIA.Application.Interfaces;

public interface IAppMetrics
{
    void RecordAiIntent(string intent);
    void RecordSriSubmission(string status);
}
