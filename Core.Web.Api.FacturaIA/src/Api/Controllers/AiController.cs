using Core.Web.Api.FacturaIA.Application.DTOs;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Web.Api.FacturaIA.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ICurrentUserService _currentUserService;

    public AiController(IAiService aiService, ICurrentUserService currentUserService)
    {
        _aiService = aiService;
        _currentUserService = currentUserService;
    }

    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] AiQueryRequestDto request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.GetTenantId();
        var result = await _aiService.ProcesarPromptAsync(request.Prompt, tenantId, cancellationToken);
        return Ok(new ApiResponse<AiQueryResponseDto>(true, result));
    }
}
