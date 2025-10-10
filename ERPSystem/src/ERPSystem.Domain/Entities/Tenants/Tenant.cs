using ERPSystem.Domain.Entities.Customers;
using ERPSystem.Domain.Entities.Invoices;
using ERPSystem.Domain.Entities.Products;
using ERPSystem.Domain.Entities.Users;

namespace ERPSystem.Domain.Entities.Tenants;

public class Tenant
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Ruc { get; set; } = string.Empty;
    public string CommercialName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string SubscriptionPlan { get; set; } = "FREE";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Configuración SRI
    public string? SriEnvironment { get; set; } // PRUEBAS o PRODUCCION
    public string? ElectronicSignaturePath { get; set; }
    public string? ElectronicSignaturePassword { get; set; }
    
    // Navegación
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
