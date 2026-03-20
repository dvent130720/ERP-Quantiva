namespace SriFacturacion.Domain.Entidades;

public sealed class Catalogo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Tipo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
