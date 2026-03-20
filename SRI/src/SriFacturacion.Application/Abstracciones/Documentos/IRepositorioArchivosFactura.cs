namespace SriFacturacion.Application.Abstracciones.Documentos;

public interface IRepositorioArchivosFactura
{
    Task<string> GuardarXmlAutorizadoAsync(Guid facturaId, string xml, CancellationToken cancellationToken);
    Task<byte[]> ObtenerXmlAutorizadoAsync(Guid facturaId, CancellationToken cancellationToken);
}
