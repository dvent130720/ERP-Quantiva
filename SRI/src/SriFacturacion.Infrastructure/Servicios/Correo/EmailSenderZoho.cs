using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Polly;
using Polly.Retry;
using SriFacturacion.Application.Abstracciones.Correo;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Servicios.Correo;

public sealed class EmailSenderZoho : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<EmailSenderZoho> _logger;
    private readonly AsyncRetryPolicy _policy;

    public EmailSenderZoho(IOptions<SmtpOptions> options, ILogger<EmailSenderZoho> logger)
    {
        _options = options.Value;
        _logger = logger;
        _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, intento => TimeSpan.FromSeconds(Math.Pow(2, intento)),
                (exception, espera, intento, _) => _logger.LogWarning(exception, "Reintento {Intento} envío correo tras esperar {Espera}", intento, espera));
    }

    public Task EnviarFacturaAsync(string destinatario, string asunto, string cuerpoHtml, byte[] pdf, byte[] xml, string nombreArchivoBase, CancellationToken cancellationToken)
    {
        return _policy.ExecuteAsync(async token =>
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(MailboxAddress.Parse(_options.Remitente));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;

            var builder = new BodyBuilder { HtmlBody = cuerpoHtml };
            builder.Attachments.Add($"{nombreArchivoBase}.pdf", pdf, ContentType.Parse("application/pdf"));
            builder.Attachments.Add($"{nombreArchivoBase}.xml", xml, ContentType.Parse("application/xml"));
            mensaje.Body = builder.ToMessageBody();

            using var cliente = new SmtpClient();
            await cliente.ConnectAsync(_options.Host, _options.Puerto, _options.UsarTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, token);
            await cliente.AuthenticateAsync(_options.Usuario, _options.Clave, token);
            await cliente.SendAsync(mensaje, token);
            await cliente.DisconnectAsync(true, token);
        }, cancellationToken);
    }
}
