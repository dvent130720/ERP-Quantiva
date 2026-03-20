namespace SriFacturacion.Application.Abstracciones.Catalogos;

public interface IProveedorCatalogos
{
    Task<decimal> ObtenerTarifaIvaAsync(string codigoImpuesto, CancellationToken cancellationToken);
}
