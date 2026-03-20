using Dapper;
using SriFacturacion.Application.Abstracciones.Persistencia;
using SriFacturacion.Domain.Entidades;
using SriFacturacion.Infrastructure.Persistencia;

namespace SriFacturacion.Infrastructure.Repositorios;

public sealed class CatalogosRepository : ICatalogoRepository
{
    private readonly IConexionFactory _conexionFactory;

    public CatalogosRepository(IConexionFactory conexionFactory)
    {
        _conexionFactory = conexionFactory;
    }

    public async Task<Catalogo?> ObtenerPorTipoCodigoAsync(string tipo, string codigo, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Catalogos WHERE Tipo = @tipo AND Codigo = @codigo AND Activo = TRUE LIMIT 1";
        using var conexion = _conexionFactory.CrearConexion();
        return await conexion.QuerySingleOrDefaultAsync<Catalogo>(new CommandDefinition(sql, new { tipo, codigo }, cancellationToken: cancellationToken));
    }
}
