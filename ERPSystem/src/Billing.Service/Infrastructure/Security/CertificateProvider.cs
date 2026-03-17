using Billing.Service.Application.Abstractions;
using Billing.Service.Domain.Exceptions;
using Billing.Service.Infrastructure.Tenants;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Billing.Service.Infrastructure.Security;

public sealed class CertificateProvider(
    ITenantCertificateClient tenantCertificateClient,
    IMemoryCache cache) : ICertificateProvider
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(7);

    public async Task<X509Certificate2> GetCertificateAsync(Guid tenantId)
    {
        var cacheKey = $"tenant-cert:{tenantId}";
        if (cache.TryGetValue<X509Certificate2>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        var secret = await tenantCertificateClient.GetCertificateSecretAsync(tenantId);
        if (secret.TenantId != tenantId)
        {
            throw new CertificateValidationException("Tenant inválido para el certificado solicitado.");
        }

        var key = ResolveMasterKey();
        byte[] p12Bytes = [];
        string password = string.Empty;

        try
        {
            p12Bytes = AesGcmEncryption.DecryptBytes(secret.P12Encrypted, key);
            password = AesGcmEncryption.DecryptString(secret.P12PasswordEncrypted, key);

            var cert = new X509Certificate2(
                p12Bytes,
                password,
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.EphemeralKeySet |
                X509KeyStorageFlags.Exportable);

            ValidateCertificate(cert);
            cache.Set(cacheKey, cert, CacheTtl);
            return cert;
        }
        catch (CryptographicException)
        {
            throw new CertificateDecryptionException("No fue posible desencriptar el .p12 o su password.");
        }
        finally
        {
            CryptographicOperations.ZeroMemory(p12Bytes);
            if (!string.IsNullOrEmpty(password))
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                CryptographicOperations.ZeroMemory(passwordBytes);
            }
        }
    }

    public static void ValidateCertificate(X509Certificate2 cert)
    {
        if (!cert.HasPrivateKey)
        {
            throw new CertificateValidationException("El certificado no contiene clave privada.");
        }

        if (DateTimeOffset.UtcNow >= cert.NotAfter)
        {
            throw new CertificateValidationException("El certificado está expirado.");
        }
    }

    private static byte[] ResolveMasterKey()
    {
        var base64 = Environment.GetEnvironmentVariable("P12_MASTER_KEY");
        if (string.IsNullOrWhiteSpace(base64))
        {
            throw new InvalidOperationException("P12_MASTER_KEY no configurada.");
        }

        var key = Convert.FromBase64String(base64);
        if (key.Length != 32)
        {
            throw new InvalidOperationException("P12_MASTER_KEY debe representar 32 bytes (AES-256). ");
        }

        return key;
    }
}
