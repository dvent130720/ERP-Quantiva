using Core.Web.Api.FacturaIA.Domain.Common;

namespace Core.Web.Api.FacturaIA.Domain.Entities;

public class LedgerEntry : BaseEntity
{
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string CuentaDebito { get; set; } = string.Empty;
    public string CuentaCredito { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "USD";
    public Guid? VentaId { get; set; }
    public Venta? Venta { get; set; }
}
