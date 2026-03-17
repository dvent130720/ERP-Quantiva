namespace Billing.Service.Domain;

public sealed class Comprobante
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = default!;
    public string ClaveAcceso { get; set; } = default!;
    public string Estado { get; set; } = "CREADA";
    public string Xml { get; set; } = string.Empty;
    public string XmlFirmado { get; set; } = string.Empty;
    public string XmlAutorizado { get; set; } = string.Empty;
    public string Errores { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
