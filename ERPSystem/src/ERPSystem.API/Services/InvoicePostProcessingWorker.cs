using System.Globalization;
using System.Text;
using ERPSystem.Domain.Entities.Invoices;
using ERPSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.API.Services;

public sealed class InvoicePostProcessingWorker : BackgroundService
{
    private readonly ILogger<InvoicePostProcessingWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IInvoiceDispatchQueue _queue;

    public InvoicePostProcessingWorker(
        ILogger<InvoicePostProcessingWorker> logger,
        IServiceScopeFactory scopeFactory,
        IInvoiceDispatchQueue queue)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de post-proceso de facturas iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            var invoiceId = await _queue.DequeueAsync(stoppingToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var invoice = await db.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.Details)
                    .FirstOrDefaultAsync(i => i.Id == invoiceId, stoppingToken);

                if (invoice is null)
                {
                    _logger.LogWarning("Factura {InvoiceId} no encontrada para post-proceso", invoiceId);
                    continue;
                }

                var pdfPath = await GenerateInvoicePdfAsync(invoice, stoppingToken);
                invoice.PdfUrl = pdfPath;
                invoice.UpdatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync(stoppingToken);

                // Mock de envío de WhatsApp
                _logger.LogInformation(
                    "Factura {InvoiceNumber} procesada. PDF: {PdfPath}. WhatsApp enviado a {CustomerPhone}",
                    invoice.FullNumber,
                    pdfPath,
                    invoice.Customer.Phone ?? "N/A");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando factura {InvoiceId}", invoiceId);
            }
        }
    }

    private static async Task<string> GenerateInvoicePdfAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "generated", "invoices");
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, $"{invoice.FullNumber}.pdf");

        var content = $"""
        FACTURA ELECTRÓNICA
        Número: {invoice.FullNumber}
        Fecha: {invoice.IssueDate:yyyy-MM-dd HH:mm}
        Cliente: {invoice.Customer.Name}
        Total: {invoice.Total.ToString("C2", CultureInfo.GetCultureInfo("es-EC"))}

        Detalle:
        {string.Join(Environment.NewLine, invoice.Details.Select(d => $"- {d.ProductName} x {d.Quantity} = {d.Total:C2}"))}

        Documento generado automáticamente para envío a WhatsApp.
        """;

        // PDF simplificado como archivo binario con contenido de texto
        await File.WriteAllBytesAsync(filePath, Encoding.UTF8.GetBytes(content), cancellationToken);
        return filePath;
    }
}
