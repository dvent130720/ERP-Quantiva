namespace Billing.Service.Infrastructure.Tenants;

public sealed record TenantCertificateSecretDto(Guid TenantId, byte[] P12Encrypted, string P12PasswordEncrypted);

public interface ITenantCertificateClient
{
    Task<TenantCertificateSecretDto> GetCertificateSecretAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
