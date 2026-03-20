using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Abstracciones.Documentos;

public interface IFirmadorXml
{
    Task<string> FirmarAsync(string xml, Certificado certificado, string claveDescifrada, CancellationToken cancellationToken);
}
