using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Domain.Entities;
using Core.Web.Api.FacturaIA.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Web.Api.FacturaIA.Infrastructure.Services;

public class SriService : ISriService
{
    private readonly AppDbContext _context;
    private readonly IVentaRepository _ventaRepository;
    private readonly IAppMetrics _metrics;
    private readonly ILogger<SriService> _logger;

    public SriService(AppDbContext context, IVentaRepository ventaRepository, IAppMetrics metrics, ILogger<SriService> logger)
    {
        _context = context;
        _ventaRepository = ventaRepository;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task<string> EnviarFacturaAsync(Guid ventaId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var venta = await _ventaRepository.GetByIdAsync(ventaId, tenantId, cancellationToken)
                   ?? throw new InvalidOperationException("La venta no existe para el tenant solicitado.");

        var certificado = await _context.Certificados
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No existe certificado cargado para este tenant.");

        var xml = GenerarXmlPlaceholder(venta);
        var signedXml = FirmarXmlPlaceholder(xml, certificado);

        _logger.LogInformation("Factura preparada para SRI. VentaId={VentaId} TenantId={TenantId} Payload={Payload}", ventaId, tenantId, signedXml);
        _metrics.RecordSriSubmission("prepared");
        return "ENVIADO";
    }

    public async Task<CertificadoDigital> SubirCertificadoAsync(IFormFile archivo, string password, string provider, Guid tenantId, CancellationToken cancellationToken = default)
    {
        await using var ms = new MemoryStream();
        await archivo.CopyToAsync(ms, cancellationToken);
        var bytes = ms.ToArray();

        var x509 = new X509Certificate2(bytes, password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);
        var cert = new CertificadoDigital
        {
            TenantId = tenantId,
            NombreArchivo = archivo.FileName,
            Archivo = bytes,
            Password = password,
            Proveedor = provider,
            ExpiraEn = x509.NotAfter
        };

        await _context.Certificados.AddAsync(cert, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return cert;
    }

    private static string GenerarXmlPlaceholder(Venta venta)
    {
        var payload = new
        {
            venta.Id,
            venta.NumeroComprobante,
            venta.Fecha,
            venta.Total,
            venta.Moneda,
            Schema = "ec.invoice.v1"
        };

        var json = JsonSerializer.Serialize(payload);
        return $"<Factura><![CDATA[{json}]]></Factura>";
    }

    private static string FirmarXmlPlaceholder(string xml, CertificadoDigital certificado)
    {
        var marker = Convert.ToBase64String(Encoding.UTF8.GetBytes(certificado.NombreArchivo));
        return $"{xml}<Signature>{marker}</Signature>";
    }
}
