using Core.Web.Api.FacturaIA.Domain.Common;

namespace Core.Web.Api.FacturaIA.Domain.Entities;

public class Auditoria : BaseEntity
{
    public string Accion { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string Endpoint { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
