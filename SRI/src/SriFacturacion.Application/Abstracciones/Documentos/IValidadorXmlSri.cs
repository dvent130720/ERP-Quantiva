namespace SriFacturacion.Application.Abstracciones.Documentos;

public interface IValidadorXmlSri
{
    Task ValidarAsync(string xml, CancellationToken cancellationToken);
}
