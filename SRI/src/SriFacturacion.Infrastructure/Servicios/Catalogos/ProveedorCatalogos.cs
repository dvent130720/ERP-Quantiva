using Microsoft.Extensions.Logging;
using SriFacturacion.Application.Abstracciones.Catalogos;
using SriFacturacion.Application.Abstracciones.Persistencia;

namespace SriFacturacion.Infrastructure.Servicios.Catalogos;

public sealed class ProveedorCatalogos : IProveedorCatalogos
{
    private readonly ICatalogoRepository _catalogoRepository;
    private readonly ILogger<ProveedorCatalogos> _logger;

    public ProveedorCatalogos(ICatalogoRepository catalogoRepository, ILogger<ProveedorCatalogos> logger)
    {
        _catalogoRepository = catalogoRepository;
        _logger = logger;
    }

    public async Task<decimal> ObtenerTarifaIvaAsync(string codigoImpuesto, CancellationToken cancellationToken)
    {
        var catalogo = await _catalogoRepository.ObtenerPorTipoCodigoAsync("IVA", codigoImpuesto, cancellationToken)
            ?? throw new InvalidOperationException($"No existe catálogo IVA para el código {codigoImpuesto}");

        if (!decimal.TryParse(catalogo.Valor, out var tarifa))
        {
            _logger.LogError("El catálogo IVA con código {Codigo} tiene un valor inválido: {Valor}", codigoImpuesto, catalogo.Valor);
            throw new InvalidOperationException("El catálogo IVA está mal configurado.");
        }

        return tarifa;
    }
}
