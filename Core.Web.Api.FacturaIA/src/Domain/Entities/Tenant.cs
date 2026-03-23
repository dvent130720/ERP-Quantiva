using Core.Web.Api.FacturaIA.Domain.Common;

namespace Core.Web.Api.FacturaIA.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string PaisCodigo { get; set; } = "EC";
    public string TimeZone { get; set; } = "America/Guayaquil";
}
