using Billing.Service.Application.Abstractions;
using Billing.Service.Domain.Exceptions;
using Billing.Service.Infrastructure.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Schema;

namespace Billing.Service.Infrastructure.Signing;

public sealed class XmlSigner : IXmlSigner
{
    private const string DsNs = SignedXml.XmlDsigNamespaceUrl;
    private const string XadesNs = "http://uri.etsi.org/01903/v1.3.2#";

    public XmlDocument Sign(XmlDocument xml, X509Certificate2 cert)
    {
        try
        {
            if (xml.DocumentElement is null)
            {
                throw new XmlSigningException("XML inválido: sin elemento raíz.");
            }

            ValidateXml(xml);
            CertificateProvider.ValidateCertificate(cert);

            xml.PreserveWhitespace = true;
            var signedXml = new SignedXml(xml)
            {
                SigningKey = cert.GetRSAPrivateKey() ?? throw new XmlSigningException("No se encontró clave RSA privada en certificado."),
                SignedInfo =
                {
                    CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl,
                    SignatureMethod = SignedXml.XmlDsigRSASHA256Url
                }
            };

            var reference = new Reference(string.Empty)
            {
                Id = "Reference-Document",
                DigestMethod = SignedXml.XmlDsigSHA256Url
            };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            signedXml.AddReference(reference);

            var signedPropertiesId = $"SignedProperties-{Guid.NewGuid():N}";
            signedXml.Signature.Id = "Signature";
            var dsObject = BuildXadesObject(cert, signedPropertiesId);
            signedXml.AddObject(dsObject);

            var signedPropsReference = new Reference($"#{signedPropertiesId}")
            {
                Type = "http://uri.etsi.org/01903#SignedProperties",
                DigestMethod = SignedXml.XmlDsigSHA256Url
            };
            signedPropsReference.AddTransform(new XmlDsigC14NTransform());
            signedXml.AddReference(signedPropsReference);

            var keyInfo = new KeyInfo();
            var x509Data = new KeyInfoX509Data(cert);
            x509Data.AddCertificate(cert);
            keyInfo.AddClause(x509Data);
            signedXml.KeyInfo = keyInfo;

            try
            {
                signedXml.ComputeSignature();
            }
            catch (CryptographicException)
            {
                signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
                signedXml.ComputeSignature();
            }

            var signature = signedXml.GetXml();
            xml.DocumentElement.AppendChild(xml.ImportNode(signature, true));
            return xml;
        }
        catch (XmlSigningException)
        {
            throw;
        }
        catch (CryptographicException ex)
        {
            throw new XmlSigningException("Error criptográfico durante la firma XAdES-BES.", ex);
        }
        catch (XmlSchemaValidationException ex)
        {
            throw new XmlSigningException("XML inválido contra XSD antes de firmar.", ex);
        }
    }

    private static DataObject BuildXadesObject(X509Certificate2 cert, string signedPropertiesId)
    {
        var objectDoc = new XmlDocument { PreserveWhitespace = true };
        var qualifyingProperties = objectDoc.CreateElement("xades", "QualifyingProperties", XadesNs);
        qualifyingProperties.SetAttribute("Target", "#Signature");

        var signedProperties = objectDoc.CreateElement("xades", "SignedProperties", XadesNs);
        signedProperties.SetAttribute("Id", signedPropertiesId);

        var signedSigProps = objectDoc.CreateElement("xades", "SignedSignatureProperties", XadesNs);
        var signingTime = objectDoc.CreateElement("xades", "SigningTime", XadesNs);
        signingTime.InnerText = DateTime.UtcNow.ToString("O");

        var signingCertificate = objectDoc.CreateElement("xades", "SigningCertificate", XadesNs);
        var certNode = objectDoc.CreateElement("xades", "Cert", XadesNs);
        var certDigest = objectDoc.CreateElement("xades", "CertDigest", XadesNs);
        var digestMethod = objectDoc.CreateElement("ds", "DigestMethod", DsNs);
        digestMethod.SetAttribute("Algorithm", SignedXml.XmlDsigSHA256Url);
        var digestValue = objectDoc.CreateElement("ds", "DigestValue", DsNs);
        digestValue.InnerText = Convert.ToBase64String(SHA256.HashData(cert.RawData));
        certDigest.AppendChild(digestMethod);
        certDigest.AppendChild(digestValue);
        certNode.AppendChild(certDigest);
        signingCertificate.AppendChild(certNode);

        signedSigProps.AppendChild(signingTime);
        signedSigProps.AppendChild(signingCertificate);

        var signedDataObjProps = objectDoc.CreateElement("xades", "SignedDataObjectProperties", XadesNs);
        var dataObjFormat = objectDoc.CreateElement("xades", "DataObjectFormat", XadesNs);
        dataObjFormat.SetAttribute("ObjectReference", "#Reference-Document");
        var mimeType = objectDoc.CreateElement("xades", "MimeType", XadesNs);
        mimeType.InnerText = "text/xml";
        dataObjFormat.AppendChild(mimeType);
        signedDataObjProps.AppendChild(dataObjFormat);

        signedProperties.AppendChild(signedSigProps);
        signedProperties.AppendChild(signedDataObjProps);
        qualifyingProperties.AppendChild(signedProperties);

        objectDoc.AppendChild(qualifyingProperties);

        return new DataObject
        {
            Data = objectDoc.ChildNodes,
            Id = "XadesObject"
        };
    }

    private static void ValidateXml(XmlDocument xml)
    {
        var xsdPath = Environment.GetEnvironmentVariable("SRI_XSD_PATH");
        if (string.IsNullOrWhiteSpace(xsdPath) || !File.Exists(xsdPath))
        {
            throw new XmlSchemaValidationException("SRI_XSD_PATH no configurado o no existe.");
        }

        var schemas = new XmlSchemaSet();
        schemas.Add(null, xsdPath);
        xml.Schemas = schemas;
        xml.Validate((_, e) => throw e.Exception ?? new XmlSchemaValidationException(e.Message));
    }
}
