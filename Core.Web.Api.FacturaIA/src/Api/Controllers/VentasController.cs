using Core.Web.Api.FacturaIA.Application.DTOs;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Application.Services;
using Core.Web.Api.FacturaIA.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web.Api.FacturaIA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ventas")]
public class VentasController : ControllerBase
{
    private readonly VentasService _ventasService;
    private readonly ICurrentUserService _currentUserService;

    public VentasController(VentasService ventasService, ICurrentUserService currentUserService)
    {
        _ventasService = ventasService;
        _currentUserService = currentUserService;
    }

    [HttpGet("total")]
    [ProducesResponseType(typeof(ApiResponse<VentasResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Total([FromQuery] VentasRequestDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.GetTenantId();
        var total = await _ventasService.ObtenerTotalAsync(dto.FechaInicio, dto.FechaFin, tenantId, cancellationToken);
        return Ok(new ApiResponse<VentasResponseDto>(true, new VentasResponseDto { Total = total }));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] VentaCreateDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.GetTenantId();
        var venta = await _ventasService.CrearVentaAsync(dto, tenantId, cancellationToken);
        return CreatedAtAction(nameof(Total), new { dto.Fecha, dto.Total }, new ApiResponse<object>(true, new { venta.Id, venta.NumeroComprobante }));
    }
}
