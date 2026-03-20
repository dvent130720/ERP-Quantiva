namespace SriFacturacion.Infrastructure.Opciones;

public sealed class SeguridadOptions
{
    public const string Seccion = "Seguridad";
    public string LlaveAesBase64 { get; set; } = string.Empty;
    public string DirectorioCertificados { get; set; } = "/app/seguridad/certificados";
    public string DirectorioAutorizados { get; set; } = "/app/seguridad/autorizados";
}
