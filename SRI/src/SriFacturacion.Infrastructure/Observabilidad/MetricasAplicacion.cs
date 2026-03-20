using Prometheus;

namespace SriFacturacion.Infrastructure.Observabilidad;

public static class MetricasAplicacion
{
    public static readonly Counter FacturasProcesadas = Metrics.CreateCounter("sri_facturas_procesadas_total", "Cantidad de facturas procesadas", new CounterConfiguration { LabelNames = new[] { "estado" } });
    public static readonly Counter Reintentos = Metrics.CreateCounter("sri_reintentos_total", "Cantidad de reintentos técnicos", new CounterConfiguration { LabelNames = new[] { "servicio" } });
    public static readonly Counter Errores = Metrics.CreateCounter("sri_errores_total", "Cantidad de errores", new CounterConfiguration { LabelNames = new[] { "servicio" } });
    public static readonly Histogram LatenciaSri = Metrics.CreateHistogram("sri_latencia_segundos", "Latencia de operaciones contra SRI", new HistogramConfiguration { LabelNames = new[] { "operacion" } });
}
