using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Core.Web.Api.Empresas.Infrastructure.Persistence;

public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _options;

    public NpgsqlConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_options.Postgres);
}
