

namespace SendSPFT.Metrics
{
    using App.Metrics;
    using App.Metrics.Counter;
    public static class MetricsRegistry
    {
        public static CounterOptions CreatedApplicationsCounter => new CounterOptions
        {
            Name = "Created Applications",
            Context = "Integration.Auth.Web.Api.SendSPFT",
            MeasurementUnit = Unit.Calls
        };
    }
}

