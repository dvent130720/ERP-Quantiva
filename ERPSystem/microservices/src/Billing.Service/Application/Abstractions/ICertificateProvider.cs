using System.Security.Cryptography.X509Certificates;

namespace Billing.Service.Application.Abstractions;

public interface ICertificateProvider
{
    Task<X509Certificate2> GetCertificateAsync(Guid tenantId);
}
