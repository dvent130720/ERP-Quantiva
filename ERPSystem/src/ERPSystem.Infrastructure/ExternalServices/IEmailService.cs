namespace ERPSystem.Infrastructure.ExternalServices;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendInvoiceEmailAsync(string to, string invoiceNumber, byte[] pdfAttachment);
}