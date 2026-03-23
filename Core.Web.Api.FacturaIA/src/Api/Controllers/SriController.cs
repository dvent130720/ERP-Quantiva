using Core.Web.Api.FacturaIA.Application.DTOs;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web.Api.FacturaIA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sri")]
public class SriController : ControllerBase
{
    private readonly ISriService _sriService;
    private readonly ICurrentUserService _currentUserService;

    public SriController(ISriService sriService, ICurrentUserService currentUserService)
    {
        _sriService = sriService;
        _currentUserService = currentUserService;
    }

    [HttpPost("certificados")]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> SubirCertificado([FromForm] IFormFile archivo, [FromForm] string password, [FromForm] string provider, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.GetTenantId();
        var cert = await _sriService.SubirCertificadoAsync(archivo, password, provider, tenantId, cancellationToken);
        return Ok(new ApiResponse<object>(true, new { cert.Id, cert.NombreArchivo, cert.ExpiraEn }));
    }

    [HttpPost("facturas/{ventaId:guid}/enviar")]
    public async Task<IActionResult> EnviarFactura(Guid ventaId, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.GetTenantId();
        var status = await _sriService.EnviarFacturaAsync(ventaId, tenantId, cancellationToken);
        return Ok(new ApiResponse<object>(true, new { ventaId, status }));
    }
}
