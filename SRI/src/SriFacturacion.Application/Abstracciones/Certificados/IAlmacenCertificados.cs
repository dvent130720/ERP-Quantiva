namespace SriFacturacion.Application.Abstracciones.Certificados;

public interface IAlmacenCertificados
{
    Task<string> GuardarAsync(string nombreArchivo, byte[] contenido, CancellationToken cancellationToken);
    Task<byte[]> ObtenerAsync(string ruta, CancellationToken cancellationToken);
}
