namespace SriFacturacion.Infrastructure.Opciones;

public sealed class SmtpOptions
{
    public const string Seccion = "Smtp";
    public string Host { get; set; } = "smtp.zoho.com";
    public int Puerto { get; set; } = 587;
    public string Usuario { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string Remitente { get; set; } = string.Empty;
    public bool UsarTls { get; set; } = true;
}
