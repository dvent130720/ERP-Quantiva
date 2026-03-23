namespace Core.Web.Api.FacturaIA.Application.DTOs;

public class LedgerEntryCreateDto
{
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string CuentaDebito { get; set; } = string.Empty;
    public string CuentaCredito { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "USD";
    public Guid? VentaId { get; set; }
}
