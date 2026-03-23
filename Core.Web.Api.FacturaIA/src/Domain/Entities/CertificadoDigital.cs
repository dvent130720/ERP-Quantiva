using Core.Web.Api.FacturaIA.Domain.Common;

namespace Core.Web.Api.FacturaIA.Domain.Entities;

public class CertificadoDigital : BaseEntity
{
    public string NombreArchivo { get; set; } = string.Empty;
    public byte[] Archivo { get; set; } = Array.Empty<byte>();
    public string Password { get; set; } = string.Empty;
    public string Proveedor { get; set; } = "SRI";
    public DateTime? ExpiraEn { get; set; }
}
