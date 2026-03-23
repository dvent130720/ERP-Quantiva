using Core.Web.Api.FacturaIA.Application.DTOs;

namespace Core.Web.Api.FacturaIA.Application.Interfaces;

public interface IAiService
{
    Task<AiQueryResponseDto> ProcesarPromptAsync(string prompt, Guid tenantId, CancellationToken cancellationToken = default);
}
