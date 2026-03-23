namespace Core.Web.Api.FacturaIA.Application.DTOs;

public class VentaCreateDto
{
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Emitida";
    public string NumeroComprobante { get; set; } = string.Empty;
    public string Moneda { get; set; } = "USD";
}
