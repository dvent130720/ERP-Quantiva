using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using SriFacturacion.Application.Abstracciones.Certificados;
using SriFacturacion.Application.Abstracciones.Documentos;
using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Infrastructure.Servicios.Documentos;

public sealed class FirmadorXmlPkcs12 : IFirmadorXml
{
    private readonly IAlmacenCertificados _almacenCertificados;

    public FirmadorXmlPkcs12(IAlmacenCertificados almacenCertificados)
    {
        _almacenCertificados = almacenCertificados;
    }

    public async Task<string> FirmarAsync(string xml, Certificado certificado, string claveDescifrada, CancellationToken cancellationToken)
    {
        var bytesCertificado = await _almacenCertificados.ObtenerAsync(certificado.RutaArchivoSeguro, cancellationToken);
        using var certificadoX509 = new X509Certificate2(bytesCertificado, claveDescifrada, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
        var documento = new XmlDocument { PreserveWhitespace = false };
        documento.LoadXml(xml);

        var signedXml = new SignedXml(documento)
        {
            SigningKey = certificadoX509.GetRSAPrivateKey()
        };

        var referencia = new Reference(string.Empty);
        referencia.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        signedXml.AddReference(referencia);
        signedXml.KeyInfo = new KeyInfo();
        signedXml.KeyInfo.AddClause(new KeyInfoX509Data(certificadoX509));
        signedXml.ComputeSignature();

        var firma = signedXml.GetXml();
        documento.DocumentElement?.AppendChild(documento.ImportNode(firma, true));
        return documento.OuterXml;
    }
}
