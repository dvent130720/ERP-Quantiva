namespace SendSPFT.CrossCutting.Metrics
{
    using Prometheus;

    using System.Globalization;
    public class MetricCollector
    {
        private static readonly string[] _labelNames = ["status_code", "method"];

        private readonly Counter _requestCounter;
        private readonly Counter _errorsEncountered;
        private readonly Histogram _responseTimeHistogram;

        public MetricCollector()
        {
            _requestCounter = Prometheus.Metrics.CreateCounter(
                "total_published_messages",
                "The total number of messages serviced by this API.");

            _errorsEncountered = Prometheus.Metrics.CreateCounter(
                "total_errors",
                "The total number of times an error has been encountered.");

            _responseTimeHistogram = Prometheus.Metrics.CreateHistogram(
                "request_duration_seconds",
                "The duration in seconds between the response to a request.",
                new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
                    LabelNames = _labelNames
                });
        }

        public void RegisterRequest() => _requestCounter.Inc();

        public void RegisterError() => _errorsEncountered.Inc();

        public void RegisterResponseTime(int statusCode, string method, TimeSpan elapsed)
        {
            _responseTimeHistogram.Labels(statusCode.ToString(CultureInfo.InvariantCulture), method).Observe(elapsed.TotalSeconds);
        }
    }
}

