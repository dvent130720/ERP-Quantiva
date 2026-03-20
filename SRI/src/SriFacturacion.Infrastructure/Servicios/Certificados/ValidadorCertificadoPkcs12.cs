using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Extensions.Logging;
using SriFacturacion.Application.Abstracciones.Certificados;
using SriFacturacion.Application.DTOs;
using SriFacturacion.Domain.ObjetosValor;

namespace SriFacturacion.Infrastructure.Servicios.Certificados;

public sealed class ValidadorCertificadoPkcs12 : IValidadorCertificado
{
    private readonly ILogger<ValidadorCertificadoPkcs12> _logger;

    public ValidadorCertificadoPkcs12(ILogger<ValidadorCertificadoPkcs12> logger)
    {
        _logger = logger;
    }

    public Task<ResultadoOperacion<SubirCertificadoResponse>> ValidarAsync(SubirCertificadoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var certificado = new X509Certificate2(
                request.Archivo,
                request.Clave,
                X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);

            if (!certificado.HasPrivateKey)
            {
                return Task.FromResult(ResultadoOperacion<SubirCertificadoResponse>.Fallo("El certificado no contiene llave privada."));
            }

            var expiracion = new DateTimeOffset(certificado.NotAfter, TimeSpan.Zero);
            if (expiracion <= DateTimeOffset.UtcNow)
            {
                return Task.FromResult(ResultadoOperacion<SubirCertificadoResponse>.Fallo("El certificado se encuentra expirado."));
            }

            var xml = new XmlDocument();
            xml.LoadXml("<prueba><valor>ok</valor></prueba>");
            var firmador = new SignedXml(xml)
            {
                SigningKey = certificado.GetRSAPrivateKey() ?? throw new CryptographicException("No se pudo obtener la llave privada RSA.")
            };
            var referencia = new Reference(string.Empty);
            referencia.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            firmador.AddReference(referencia);
            firmador.KeyInfo = new KeyInfo();
            firmador.KeyInfo.AddClause(new KeyInfoX509Data(certificado));
            firmador.ComputeSignature();
            xml.DocumentElement?.AppendChild(xml.ImportNode(firmador.GetXml(), true));

            var respuesta = new SubirCertificadoResponse(Guid.NewGuid(), expiracion, certificado.Thumbprint ?? string.Empty);
            return Task.FromResult(ResultadoOperacion<SubirCertificadoResponse>.Ok(respuesta, "Certificado válido"));
        }
        catch (CryptographicException exception)
        {
            _logger.LogWarning(exception, "El certificado no pudo validarse por clave inválida o archivo corrupto.");
            return Task.FromResult(ResultadoOperacion<SubirCertificadoResponse>.Fallo("Archivo .p12 inválido, algoritmo no soportado o clave incorrecta."));
        }
    }
}
