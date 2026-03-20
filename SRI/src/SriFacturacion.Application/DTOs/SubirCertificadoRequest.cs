namespace SriFacturacion.Application.DTOs;

public sealed class SubirCertificadoRequest
{
    public string NombreArchivo { get; set; } = string.Empty;
    public byte[] Archivo { get; set; } = Array.Empty<byte>();
    public string Clave { get; set; } = string.Empty;
    public string RucTitular { get; set; } = string.Empty;
}
