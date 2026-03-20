using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using SriFacturacion.Application.Abstracciones.Bus;
using SriFacturacion.Application.Abstracciones.Facturacion;
using SriFacturacion.Application.Configuracion;
using SriFacturacion.Domain.Eventos;
using SriFacturacion.Infrastructure.Observabilidad;

namespace SriFacturacion.Worker;

public sealed class ProcesadorFacturasBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumidorEventos _consumidorEventos;
    private readonly ILogger<ProcesadorFacturasBackgroundService> _logger;

    public ProcesadorFacturasBackgroundService(IServiceScopeFactory scopeFactory, IConsumidorEventos consumidorEventos, ILogger<ProcesadorFacturasBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _consumidorEventos = consumidorEventos;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumidorEventos.IniciarConsumoAsync<EventoFacturaCreada>(ColasMensajeria.FacturasPendientes, async (evento, correlationId) =>
        {
            using var scope = _scopeFactory.CreateScope();
            using var actividad = MetricasAplicacion.LatenciaSri.WithLabels("proceso_factura").NewTimer();
            try
            {
                var servicioFacturas = scope.ServiceProvider.GetRequiredService<IServicioFacturas>();
                await servicioFacturas.ProcesarAsync(evento.FacturaId, string.IsNullOrWhiteSpace(evento.CorrelationId) ? correlationId : evento.CorrelationId, stoppingToken);
                MetricasAplicacion.FacturasProcesadas.WithLabels("procesada").Inc();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando factura {FacturaId}", evento.FacturaId);
                MetricasAplicacion.Errores.WithLabels("worker-facturacion").Inc();
                return false;
            }
        }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
