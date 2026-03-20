namespace SriFacturacion.Domain.Entidades;

public sealed class FacturaItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FacturaId { get; set; }
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PorcentajeDescuento { get; set; }
    public decimal BaseImponible { get; set; }
    public string CodigoImpuesto { get; set; } = string.Empty;
    public decimal TarifaImpuesto { get; set; }
    public decimal ValorImpuesto { get; set; }
    public decimal Total => Math.Round((Cantidad * PrecioUnitario) - PorcentajeDescuento + ValorImpuesto, 2, MidpointRounding.AwayFromZero);
}
