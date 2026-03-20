namespace SriFacturacion.Application.Abstracciones.Sri;

public interface IClienteSriSoap
{
    Task<RespuestaRecepcionSri> EnviarComprobanteAsync(string xmlBase64, CancellationToken cancellationToken);
    Task<RespuestaAutorizacionSri> ConsultarAutorizacionAsync(string claveAcceso, CancellationToken cancellationToken);
}

public sealed record RespuestaRecepcionSri(bool Exitoso, string Estado, string Mensaje);
public sealed record RespuestaAutorizacionSri(bool Autorizado, string Estado, string Mensaje, string? XmlAutorizado, DateTimeOffset? FechaAutorizacion);
