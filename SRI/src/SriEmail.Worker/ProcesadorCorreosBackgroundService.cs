using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SriFacturacion.Application.Abstracciones.Bus;
using SriFacturacion.Application.Abstracciones.Facturacion;
using SriFacturacion.Application.Configuracion;
using SriFacturacion.Application.DTOs;
using SriFacturacion.Domain.Eventos;
using SriFacturacion.Infrastructure.Observabilidad;

namespace SriEmail.Worker;

public sealed class ProcesadorCorreosBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumidorEventos _consumidorEventos;
    private readonly ILogger<ProcesadorCorreosBackgroundService> _logger;

    public ProcesadorCorreosBackgroundService(IServiceScopeFactory scopeFactory, IConsumidorEventos consumidorEventos, ILogger<ProcesadorCorreosBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _consumidorEventos = consumidorEventos;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumidorEventos.IniciarConsumoAsync<EventoFacturaAutorizada>(ColasMensajeria.FacturasAutorizadas, async (evento, correlationId) =>
        {
            using var scope = _scopeFactory.CreateScope();
            try
            {
                var servicioFacturas = scope.ServiceProvider.GetRequiredService<IServicioFacturas>();
                await servicioFacturas.EnviarCorreoAsync(new EventoFacturaAutorizadaDto(evento.FacturaId, evento.CorrelationId, evento.ClaveAcceso, evento.CorreoCliente, evento.FechaAutorizacion), stoppingToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de factura {FacturaId}", evento.FacturaId);
                MetricasAplicacion.Errores.WithLabels("worker-email").Inc();
                return false;
            }
        }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
