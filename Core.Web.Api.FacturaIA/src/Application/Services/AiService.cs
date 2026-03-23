using System.Globalization;
using System.Text.RegularExpressions;
using Core.Web.Api.FacturaIA.Application.DTOs;
using Core.Web.Api.FacturaIA.Application.Interfaces;

namespace Core.Web.Api.FacturaIA.Application.Services;

public class AiService : IAiService
{
    private readonly VentasService _ventasService;
    private readonly IAppMetrics _metrics;

    public AiService(VentasService ventasService, IAppMetrics metrics)
    {
        _ventasService = ventasService;
        _metrics = metrics;
    }

    public async Task<AiQueryResponseDto> ProcesarPromptAsync(string prompt, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var normalized = prompt.Trim().ToLowerInvariant();

        if (normalized.Contains("vend") || normalized.Contains("venta"))
        {
            var range = ResolveRange(normalized);
            var total = await _ventasService.ObtenerTotalAsync(range.start, range.end, tenantId, cancellationToken);
            _metrics.RecordAiIntent("ventas_total");
            return new AiQueryResponseDto
            {
                Intent = "ventas_total",
                Respuesta = $"El total de ventas entre {range.start:yyyy-MM-dd} y {range.end:yyyy-MM-dd} es {total.ToString("C", CultureInfo.InvariantCulture)}."
            };
        }

        _metrics.RecordAiIntent("fallback");
        return new AiQueryResponseDto
        {
            Intent = "fallback",
            Respuesta = "No pude mapear el intent todavía. Intenta con una consulta de ventas o amplía el orquestador con nuevos módulos."
        };
    }

    private static (DateTime start, DateTime end) ResolveRange(string prompt)
    {
        var now = DateTime.UtcNow.Date;
        if (prompt.Contains("hoy")) return (now, now.AddDays(1).AddTicks(-1));
        if (prompt.Contains("mes"))
        {
            var start = new DateTime(now.Year, now.Month, 1);
            return (start, start.AddMonths(1).AddTicks(-1));
        }

        var matches = Regex.Matches(prompt, @"\d{4}-\d{2}-\d{2}");
        if (matches.Count >= 2 && DateTime.TryParse(matches[0].Value, out var startDate) && DateTime.TryParse(matches[1].Value, out var endDate))
        {
            return (startDate, endDate.AddDays(1).AddTicks(-1));
        }

        return (now.AddDays(-30), now.AddDays(1).AddTicks(-1));
    }
}
