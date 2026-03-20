using System.Data.Common;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Web.Api.Empresas.Infrastructure.Persistence;

public sealed class DatabaseInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        IDbConnectionFactory dbConnectionFactory,
        IHostEnvironment hostEnvironment,
        ILogger<DatabaseInitializer> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var ddlPath = Path.Combine(_hostEnvironment.ContentRootPath, "..", "..", "scripts", "init-ddl.sql");
        var fullPath = Path.GetFullPath(ddlPath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"No se encontró el archivo DDL: {fullPath}");
        }

        var ddl = await File.ReadAllTextAsync(fullPath, cancellationToken);

        await using var connection = (DbConnection)_dbConnectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(ddl, cancellationToken: cancellationToken));

        _logger.LogInformation("DDL ejecutado correctamente desde {DdlPath}", fullPath);
    }
}
