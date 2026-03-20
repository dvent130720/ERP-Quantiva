using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Persistencia;

public interface IConexionFactory
{
    IDbConnection CrearConexion();
}

public sealed class ConexionFactory : IConexionFactory
{
    private readonly PostgresOptions _options;

    public ConexionFactory(IOptions<PostgresOptions> options)
    {
        _options = options.Value;
    }

    public IDbConnection CrearConexion() => new NpgsqlConnection(_options.CadenaConexion);
}
