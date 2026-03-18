using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;

namespace Quantiva.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/audits")]
public sealed class AuditsController(IAuditService auditService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditTrailDto>>> GetLatest([FromQuery] int take = 100, CancellationToken cancellationToken = default)
        => Ok(await auditService.GetLatestAsync(take, cancellationToken));
}
