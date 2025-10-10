using ERPSystem.Domain.Entities.Tenants;

namespace ERPSystem.Domain.Entities.Products;

public class Product
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public string TaxType { get; set; } = "IVA"; // IVA, ICE, IRBPNR, EXENTO
    public decimal TaxPercentage { get; set; } = 15; // 15% IVA por defecto
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navegación
    public Tenant Tenant { get; set; } = null!;
}