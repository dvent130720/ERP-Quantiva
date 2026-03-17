using System.Net.Http.Json;

namespace Billing.Service.Infrastructure.Tenants;

public sealed class TenantCertificateClient(HttpClient httpClient) : ITenantCertificateClient
{
    public async Task<TenantCertificateSecretDto> GetCertificateSecretAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/tenants/{tenantId}/certificate", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Tenant {tenantId} no encontrado o sin certificado.");
        }

        var payload = await response.Content.ReadFromJsonAsync<TenantCertificateSecretDto>(cancellationToken: cancellationToken);
        return payload ?? throw new InvalidOperationException("Respuesta de certificado inválida.");
    }
}
