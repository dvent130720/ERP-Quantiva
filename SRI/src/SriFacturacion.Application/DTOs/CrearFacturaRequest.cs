namespace SriFacturacion.Application.DTOs;

public sealed class CrearFacturaRequest
{
    public string NumeroDocumento { get; set; } = string.Empty;
    public string RucEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string CorreoCliente { get; set; } = string.Empty;
    public string IdentificacionCliente { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public string Moneda { get; set; } = "USD";
    public Guid CertificadoId { get; set; }
    public List<CrearFacturaItemRequest> Items { get; set; } = new();
}

public sealed class CrearFacturaItemRequest
{
    public string CodigoPrincipal { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal PorcentajeDescuento { get; set; }
    public string CodigoImpuesto { get; set; } = string.Empty;
}
