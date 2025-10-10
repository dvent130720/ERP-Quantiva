using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using ERPSystem.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ERPSystem.Infrastructure.Services.SRI;

public class SriInvoiceService : ISriInvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SriInvoiceService> _logger;
    private readonly IConfiguration _configuration;

    public SriInvoiceService(
        ApplicationDbContext context,
        ILogger<SriInvoiceService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> GenerateAccessKey(Guid invoiceId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Tenant)
            .FirstOrDefaultAsync(i => i.Id == invoiceId)
            ?? throw new Exception("Factura no encontrada");

        // Formato de clave de acceso (49 dígitos):
        // ddmmaaaa + tipo_comprobante(2) + ruc(13) + ambiente(1) + serie(6) + secuencial(9) + codigo_numerico(8) + tipo_emision(1)
        
        var fecha = invoice.IssueDate.ToString("ddMMyyyy");
        var tipoComprobante = "01"; // 01 = Factura
        var ruc = invoice.Tenant.Ruc.PadLeft(13, '0');
        var ambiente = invoice.Tenant.SriEnvironment == "PRODUCCION" ? "2" : "1";
        var serie = invoice.EstablishmentCode + invoice.EmissionPointCode;
        var secuencial = invoice.SequentialNumber.PadLeft(9, '0');
        var codigoNumerico = new Random().Next(10000000, 99999999).ToString();
        var tipoEmision = "1"; // 1 = Emisión normal

        // Construir clave sin dígito verificador
        var claveBase = $"{fecha}{tipoComprobante}{ruc}{ambiente}{serie}{secuencial}{codigoNumerico}{tipoEmision}";
        
        // Calcular dígito verificador (Módulo 11)
        var digitoVerificador = CalculateModulo11(claveBase);
        
        var claveAcceso = claveBase + digitoVerificador;
        
        _logger.LogInformation($"Clave de acceso generada: {claveAcceso}");
        
        return claveAcceso;
    }

    public async Task<string> GenerateXml(Guid invoiceId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Tenant)
            .Include(i => i.Customer)
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == invoiceId)
            ?? throw new Exception("Factura no encontrada");

        if (string.IsNullOrEmpty(invoice.AccessKey))
        {
            invoice.AccessKey = await GenerateAccessKey(invoiceId);
            await _context.SaveChangesAsync();
        }

        var xml = new XDocument(
            new XDeclaration("1.0", "UTF-8", "yes"),
            new XElement("factura",
                new XAttribute("id", "comprobante"),
                new XAttribute("version", "1.1.0"),
                
                // Información Tributaria
                new XElement("infoTributaria",
                    new XElement("ambiente", invoice.Tenant.SriEnvironment == "PRODUCCION" ? "2" : "1"),
                    new XElement("tipoEmision", "1"),
                    new XElement("razonSocial", invoice.Tenant.CompanyName),
                    new XElement("nombreComercial", invoice.Tenant.CommercialName),
                    new XElement("ruc", invoice.Tenant.Ruc),
                    new XElement("claveAcceso", invoice.AccessKey),
                    new XElement("codDoc", "01"),
                    new XElement("estab", invoice.EstablishmentCode),
                    new XElement("ptoEmi", invoice.EmissionPointCode),
                    new XElement("secuencial", invoice.SequentialNumber.PadLeft(9, '0')),
                    new XElement("dirMatriz", invoice.Tenant.Address)
                ),
                
                // Información de la Factura
                new XElement("infoFactura",
                    new XElement("fechaEmision", invoice.IssueDate.ToString("dd/MM/yyyy")),
                    new XElement("dirEstablecimiento", invoice.Tenant.Address),
                    new XElement("obligadoContabilidad", "SI"),
                    new XElement("tipoIdentificacionComprador", 
                        invoice.Customer.IdentificationType == "RUC" ? "04" : 
                        invoice.Customer.IdentificationType == "CEDULA" ? "05" : "06"),
                    new XElement("razonSocialComprador", invoice.Customer.Name),
                    new XElement("identificacionComprador", invoice.Customer.Identification),
                    new XElement("totalSinImpuestos", invoice.Subtotal.ToString("F2")),
                    new XElement("totalDescuento", invoice.Discount.ToString("F2")),
                    new XElement("totalConImpuestos",
                        new XElement("totalImpuesto",
                            new XElement("codigo", "2"), // 2 = IVA
                            new XElement("codigoPorcentaje", "2"), // 2 = 15%
                            new XElement("baseImponible", invoice.Subtotal.ToString("F2")),
                            new XElement("valor", invoice.TaxAmount.ToString("F2"))
                        )
                    ),
                    new XElement("propina", "0.00"),
                    new XElement("importeTotal", invoice.Total.ToString("F2")),
                    new XElement("moneda", "DOLAR"),
                    new XElement("pagos",
                        new XElement("pago",
                            new XElement("formaPago", "01"), // 01 = Sin utilización del sistema financiero
                            new XElement("total", invoice.Total.ToString("F2")),
                            new XElement("plazo", "0"),
                            new XElement("unidadTiempo", "dias")
                        )
                    )
                ),
                
                // Detalles
                new XElement("detalles",
                    invoice.Details.Select(d => new XElement("detalle",
                        new XElement("codigoPrincipal", d.ProductCode),
                        new XElement("descripcion", d.ProductName),
                        new XElement("cantidad", d.Quantity.ToString("F4")),
                        new XElement("precioUnitario", d.UnitPrice.ToString("F6")),
                        new XElement("descuento", d.Discount.ToString("F2")),
                        new XElement("precioTotalSinImpuesto", d.Subtotal.ToString("F2")),
                        new XElement("impuestos",
                            new XElement("impuesto",
                                new XElement("codigo", "2"), // IVA
                                new XElement("codigoPorcentaje", "2"), // 15%
                                new XElement("tarifa", d.TaxPercentage.ToString("F2")),
                                new XElement("baseImponible", d.Subtotal.ToString("F2")),
                                new XElement("valor", d.TaxAmount.ToString("F2"))
                            )
                        )
                    ))
                ),
                
                // Información Adicional
                new XElement("infoAdicional",
                    new XElement("campoAdicional", 
                        new XAttribute("nombre", "Email"), 
                        invoice.Customer.Email),
                    new XElement("campoAdicional", 
                        new XAttribute("nombre", "Teléfono"), 
                        invoice.Customer.Phone)
                )
            )
        );

        return xml.ToString();
    }

    public async Task<string> SignXml(string xml, string certificatePath, string password)
    {
        try
        {
            // Cargar certificado digital (.p12)
            var certificate = new X509Certificate2(certificatePath, password);
            
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            
            // TODO: Implementar firma digital XML con RSA
            // Por ahora retornamos el XML sin firmar para pruebas
            _logger.LogWarning("Firma digital no implementada - retornando XML sin firmar");
            
            return xml;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error firmando XML: {ex.Message}");
            throw new Exception($"Error al firmar XML: {ex.Message}");
        }
    }

    public async Task<SriRecepcionResponse> SendToSri(string signedXml)
    {
        try
        {
            // TODO: Implementar cliente SOAP para enviar al SRI
            // Por ahora simulamos respuesta exitosa
            _logger.LogInformation("Enviando comprobante al SRI (simulado)...");
            
            await Task.Delay(1000); // Simular latencia de red
            
            return new SriRecepcionResponse(
                Success: true,
                Message: "Comprobante recibido correctamente",
                State: "RECIBIDA",
                Errors: new List<string>()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error enviando a SRI: {ex.Message}");
            return new SriRecepcionResponse(
                Success: false,
                Message: ex.Message,
                State: "ERROR",
                Errors: new List<string> { ex.Message }
            );
        }
    }

    public async Task<SriAutorizacionResponse> CheckAuthorization(string accessKey)
    {
        try
        {
            // TODO: Implementar cliente SOAP para consultar autorización
            _logger.LogInformation($"Consultando autorización SRI para: {accessKey}");
            
            await Task.Delay(500);
            
            return new SriAutorizacionResponse(
                Success: true,
                AuthorizationNumber: accessKey,
                AuthorizationDate: DateTime.UtcNow,
                State: "AUTORIZADO",
                Errors: new List<string>()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error consultando autorización: {ex.Message}");
            return new SriAutorizacionResponse(
                Success: false,
                AuthorizationNumber: null,
                AuthorizationDate: null,
                State: "ERROR",
                Errors: new List<string> { ex.Message }
            );
        }
    }

    private int CalculateModulo11(string claveBase)
    {
        int factor = 7;
        int suma = 0;
        
        for (int i = 0; i < claveBase.Length; i++)
        {
            int digito = int.Parse(claveBase[i].ToString());
            suma += digito * factor;
            factor = factor == 2 ? 7 : factor - 1;
        }
        
        int residuo = suma % 11;
        int resultado = 11 - residuo;
        
        return resultado == 11 ? 0 : (resultado == 10 ? 1 : resultado);
    }
}