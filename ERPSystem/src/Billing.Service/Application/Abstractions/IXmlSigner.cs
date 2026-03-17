using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Billing.Service.Application.Abstractions;

public interface IXmlSigner
{
    XmlDocument Sign(XmlDocument xml, X509Certificate2 cert);
}
