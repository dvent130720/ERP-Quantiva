namespace SriFacturacion.Domain.Entidades;

public sealed class Certificado
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivoSeguro { get; set; } = string.Empty;
    public string ClaveCifrada { get; set; } = string.Empty;
    public string Thumbprint { get; set; } = string.Empty;
    public string RucTitular { get; set; } = string.Empty;
    public DateTimeOffset FechaExpiracion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTimeOffset FechaCreacion { get; set; } = DateTimeOffset.UtcNow;
}
