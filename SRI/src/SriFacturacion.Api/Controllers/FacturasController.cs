using Microsoft.AspNetCore.Mvc;
using SriFacturacion.Application.Abstracciones.Facturacion;
using SriFacturacion.Application.DTOs;

namespace SriFacturacion.Api.Controllers;

[ApiController]
[Route("sri/facturas")]
public sealed class FacturasController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CrearFacturaResponse), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CrearFactura(
        [FromServices] IServicioFacturas servicioFacturas,
        [FromBody] CrearFacturaRequest request,
        CancellationToken cancellationToken)
    {
        var respuesta = await servicioFacturas.CrearAsync(request, HttpContext.TraceIdentifier, cancellationToken);
        return AcceptedAtAction(nameof(ConsultarEstado), new { id = respuesta.FacturaId }, respuesta);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FacturaEstadoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarEstado(
        Guid id,
        [FromServices] IServicioFacturas servicioFacturas,
        CancellationToken cancellationToken)
    {
        var estado = await servicioFacturas.ObtenerEstadoAsync(id, cancellationToken);
        return estado is null ? NotFound() : Ok(estado);
    }
}
