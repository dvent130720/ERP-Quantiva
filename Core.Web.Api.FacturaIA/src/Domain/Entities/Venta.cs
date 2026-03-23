using Core.Web.Api.FacturaIA.Domain.Common;

namespace Core.Web.Api.FacturaIA.Domain.Entities;

public class Venta : BaseEntity
{
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string NumeroComprobante { get; set; } = string.Empty;
    public string Moneda { get; set; } = "USD";
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
}
