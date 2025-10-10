using ERPSystem.Domain.Entities.Customers;
using ERPSystem.Domain.Entities.Tenants;

namespace ERPSystem.Domain.Entities.Invoices;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    
    // Numeración
    public string EstablishmentCode { get; set; } = "001"; // 001
    public string EmissionPointCode { get; set; } = "001"; // 001
    public string SequentialNumber { get; set; } = string.Empty; // 000000001
    public string FullNumber { get; set; } = string.Empty; // 001-001-000000001
    
    // SRI
    public string? AccessKey { get; set; } // Clave de acceso de 49 dígitos
    public string? AuthorizationNumber { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public string Status { get; set; } = "DRAFT"; // DRAFT, SENT, AUTHORIZED, REJECTED, CANCELLED
    
    // Fechas
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    
    // Montos
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    
    // XML y Archivos
    public string? XmlContent { get; set; }
    public string? XmlSignedContent { get; set; }
    public string? PdfUrl { get; set; }
    
    // Respuesta SRI
    public string? SriResponse { get; set; }
    public string? SriErrors { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navegación
    public Tenant Tenant { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}