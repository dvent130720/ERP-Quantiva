namespace SriFacturacion.Application.Abstracciones.Correo;

public interface IEmailSender
{
    Task EnviarFacturaAsync(string destinatario, string asunto, string cuerpoHtml, byte[] pdf, byte[] xml, string nombreArchivoBase, CancellationToken cancellationToken);
}
