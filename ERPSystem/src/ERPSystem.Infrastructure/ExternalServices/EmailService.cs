using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERPSystem.Infrastructure.ExternalServices;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        // TODO: Implementar con SendGrid, AWS SES, o SMTP
        _logger.LogInformation("Enviando email a {To} con asunto: {Subject}", to, subject);
        
        // Simulación
        await Task.Delay(100);
        
        _logger.LogInformation("Email enviado exitosamente");
    }

    public async Task SendInvoiceEmailAsync(string to, string invoiceNumber, byte[] pdfAttachment)
    {
        var subject = $"Factura Electrónica #{invoiceNumber}";
        var body = $@"
            <h2>Factura Electrónica</h2>
            <p>Estimado cliente,</p>
            <p>Adjunto encontrará su factura electrónica número <strong>{invoiceNumber}</strong>.</p>
            <p>Gracias por su preferencia.</p>
        ";

        await SendEmailAsync(to, subject, body, true);
    }
}