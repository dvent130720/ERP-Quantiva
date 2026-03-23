using Core.Web.Api.FacturaIA.Application.DTOs;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Application.Services;
using Core.Web.Api.FacturaIA.Domain.Entities;
using Core.Web.Api.FacturaIA.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web.Api.FacturaIA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ledger")]
public class LedgerController : ControllerBase
{
    private readonly LedgerService _ledgerService;
    private readonly ICurrentUserService _currentUserService;

    public LedgerController(LedgerService ledgerService, ICurrentUserService currentUserService)
    {
        _ledgerService = ledgerService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] LedgerEntryCreateDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.GetTenantId();
        var entry = await _ledgerService.CrearAsync(dto, tenantId, cancellationToken);
        return Ok(new ApiResponse<LedgerEntry>(true, entry));
    }

    [HttpGet]
    public async Task<IActionResult> Obtener([FromQuery] VentasRequestDto dto, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.GetTenantId();
        var entries = await _ledgerService.ObtenerRangoAsync(dto.FechaInicio, dto.FechaFin, tenantId, cancellationToken);
        return Ok(new ApiResponse<IReadOnlyList<LedgerEntry>>(true, entries));
    }
}
