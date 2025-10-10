using ERPSystem.Domain.Entities.Invoices;
using ERPSystem.Domain.Entities.Tenants;

namespace ERPSystem.Domain.Entities.Customers;

public class Customer
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string IdentificationType { get; set; } = string.Empty; // RUC, CEDULA, PASAPORTE
    public string Identification { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navegación
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}