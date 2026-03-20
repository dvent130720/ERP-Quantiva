using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SriFacturacion.Infrastructure.Persistencia;

namespace SriFacturacion.Infrastructure.Servicios.Migraciones;

public sealed class HostedMigracionInicial : IHostedService
{
    private readonly IConexionFactory _conexionFactory;
    private readonly ILogger<HostedMigracionInicial> _logger;

    public HostedMigracionInicial(IConexionFactory conexionFactory, ILogger<HostedMigracionInicial> logger)
    {
        _conexionFactory = conexionFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var conexion = _conexionFactory.CrearConexion();
        if (conexion is System.Data.Common.DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
        }
        else
        {
            conexion.Open();
        }
        const string tablaControl = "CREATE TABLE IF NOT EXISTS __Migraciones (Id TEXT PRIMARY KEY, Fecha TIMESTAMPTZ NOT NULL DEFAULT NOW());";
        await conexion.ExecuteAsync(new CommandDefinition(tablaControl, cancellationToken: cancellationToken));

        var scripts = new[]
        {
            (Id: "001_inicial", Sql: await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Recursos", "Migraciones", "001_inicial.sql"), cancellationToken))
        };

        foreach (var script in scripts)
        {
            var existe = await conexion.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(1) FROM __Migraciones WHERE Id = @Id", new { script.Id }, cancellationToken: cancellationToken));
            if (existe > 0)
            {
                continue;
            }

            _logger.LogInformation("Ejecutando migración {Migracion}", script.Id);
            using var transaccion = conexion.BeginTransaction();
            await conexion.ExecuteAsync(new CommandDefinition(script.Sql, transaction: transaccion, cancellationToken: cancellationToken));
            await conexion.ExecuteAsync(new CommandDefinition("INSERT INTO __Migraciones (Id) VALUES (@Id)", new { script.Id }, transaccion, cancellationToken: cancellationToken));
            transaccion.Commit();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
