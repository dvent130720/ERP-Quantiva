namespace SriFacturacion.Infrastructure.Opciones;

public sealed class SriSoapOptions
{
    public const string Seccion = "SriSoap";
    public string UrlRecepcion { get; set; } = string.Empty;
    public string UrlAutorizacion { get; set; } = string.Empty;
    public bool AmbientePruebas { get; set; } = true;
    public int TiempoEsperaSegundos { get; set; } = 30;
}
