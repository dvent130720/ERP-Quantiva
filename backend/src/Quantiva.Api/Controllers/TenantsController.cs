using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;

namespace Quantiva.Api.Controllers;

[ApiController]
[Authorize(Roles = "PlatformAdmin")]
[Route("api/platform/tenants")]
public sealed class TenantsController(ITenantAdminService tenantAdminService, ITenantContext tenantContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TenantDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await tenantAdminService.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TenantDto>> Create([FromBody] CreateTenantDto request, CancellationToken cancellationToken)
        => Ok(await tenantAdminService.CreateAsync(request, cancellationToken));

    [HttpGet("current")]
    [AllowAnonymous]
    public IActionResult GetCurrent()
    {
        if (!tenantContext.IsAvailable)
        {
            return NotFound(new { message = "No se resolvió tenant para la petición actual." });
        }

        return Ok(new
        {
            tenantContext.TenantId,
            tenantContext.TenantSlug,
            tenantContext.TenantName
        });
    }
}
