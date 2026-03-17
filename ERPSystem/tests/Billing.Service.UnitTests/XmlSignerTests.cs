using Billing.Service.Domain.Exceptions;
using Billing.Service.Infrastructure.Security;
using Billing.Service.Infrastructure.Signing;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Billing.Service.UnitTests;

public sealed class XmlSignerTests
{
    [Fact]
    public void ValidateCertificate_Should_Throw_When_NoPrivateKey()
    {
        using var certWithKey = CreateCertificate();
        var publicOnly = new X509Certificate2(certWithKey.Export(X509ContentType.Cert));

        Assert.Throws<CertificateValidationException>(() => CertificateProvider.ValidateCertificate(publicOnly));
    }

    [Fact]
    public void Sign_Should_Append_Signature_And_XadesNodes()
    {
        Environment.SetEnvironmentVariable("SRI_XSD_PATH", CreateSchemaFile());
        using var cert = CreateCertificate();
        var xml = new XmlDocument();
        xml.LoadXml("<factura><infoTributaria><claveAcceso>1</claveAcceso></infoTributaria></factura>");

        var signed = new XmlSigner().Sign(xml, cert);

        Assert.NotNull(signed.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#").Item(0));
        Assert.NotNull(signed.GetElementsByTagName("QualifyingProperties", "http://uri.etsi.org/01903/v1.3.2#").Item(0));
    }

    private static X509Certificate2 CreateCertificate()
    {
        using var rsa = RSA.Create(2048);
        var req = new CertificateRequest("CN=tenant-test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
    }

    private static string CreateSchemaFile()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path,
            """
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
              <xs:element name="factura">
                <xs:complexType>
                  <xs:sequence>
                    <xs:element name="infoTributaria" minOccurs="1" maxOccurs="1">
                      <xs:complexType>
                        <xs:sequence>
                          <xs:any minOccurs="0" maxOccurs="unbounded" processContents="lax"/>
                        </xs:sequence>
                      </xs:complexType>
                    </xs:element>
                  </xs:sequence>
                </xs:complexType>
              </xs:element>
            </xs:schema>
            """);
        return path;
    }
}
