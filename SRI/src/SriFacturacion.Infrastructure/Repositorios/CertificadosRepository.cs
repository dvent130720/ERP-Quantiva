using Dapper;
using SriFacturacion.Application.Abstracciones.Persistencia;
using SriFacturacion.Domain.Entidades;
using SriFacturacion.Infrastructure.Persistencia;

namespace SriFacturacion.Infrastructure.Repositorios;

public sealed class CertificadosRepository : ICertificadoRepository
{
    private readonly IConexionFactory _conexionFactory;

    public CertificadosRepository(IConexionFactory conexionFactory)
    {
        _conexionFactory = conexionFactory;
    }

    public async Task CrearAsync(Certificado certificado, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO Certificados (Id, NombreArchivo, RutaArchivoSeguro, ClaveCifrada, Thumbprint, RucTitular, FechaExpiracion, Activo, FechaCreacion)
            VALUES (@Id, @NombreArchivo, @RutaArchivoSeguro, @ClaveCifrada, @Thumbprint, @RucTitular, @FechaExpiracion, @Activo, @FechaCreacion);
            """;

        using var conexion = _conexionFactory.CrearConexion();
        await conexion.ExecuteAsync(new CommandDefinition(sql, certificado, cancellationToken: cancellationToken));
    }

    public async Task<Certificado?> ObtenerPorIdAsync(Guid certificadoId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Certificados WHERE Id = @certificadoId";
        using var conexion = _conexionFactory.CrearConexion();
        return await conexion.QuerySingleOrDefaultAsync<Certificado>(new CommandDefinition(sql, new { certificadoId }, cancellationToken: cancellationToken));
    }
}
