using Microsoft.AspNetCore.Http;
using Core.Web.Api.FacturaIA.Domain.Entities;

namespace Core.Web.Api.FacturaIA.Application.Interfaces;

public interface ISriService
{
    Task<string> EnviarFacturaAsync(Guid ventaId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<CertificadoDigital> SubirCertificadoAsync(IFormFile archivo, string password, string provider, Guid tenantId, CancellationToken cancellationToken = default);
}
