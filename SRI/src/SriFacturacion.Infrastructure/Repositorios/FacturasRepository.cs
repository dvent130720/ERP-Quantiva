using Dapper;
using SriFacturacion.Application.Abstracciones.Persistencia;
using SriFacturacion.Domain.Entidades;
using SriFacturacion.Domain.Enumeradores;
using SriFacturacion.Infrastructure.Persistencia;

namespace SriFacturacion.Infrastructure.Repositorios;

public sealed class FacturasRepository : IFacturaRepository
{
    private readonly IConexionFactory _conexionFactory;

    public FacturasRepository(IConexionFactory conexionFactory)
    {
        _conexionFactory = conexionFactory;
    }

    public async Task CrearAsync(Factura factura, CancellationToken cancellationToken)
    {
        const string sqlFactura = """
            INSERT INTO Facturas (Id, NumeroDocumento, RucEmisor, RazonSocialEmisor, CorreoCliente, IdentificacionCliente, NombreCliente, Moneda, Subtotal, Impuestos, Total, ClaveAcceso, XmlGenerado, Estado, MensajeEstado, FechaCreacion, CertificadoId)
            VALUES (@Id, @NumeroDocumento, @RucEmisor, @RazonSocialEmisor, @CorreoCliente, @IdentificacionCliente, @NombreCliente, @Moneda, @Subtotal, @Impuestos, @Total, @ClaveAcceso, @XmlGenerado, @Estado, @MensajeEstado, @FechaCreacion, @CertificadoId);
            """;

        const string sqlItem = """
            INSERT INTO FacturaItems (Id, FacturaId, CodigoPrincipal, Descripcion, Cantidad, PrecioUnitario, PorcentajeDescuento, BaseImponible, CodigoImpuesto, TarifaImpuesto, ValorImpuesto)
            VALUES (@Id, @FacturaId, @CodigoPrincipal, @Descripcion, @Cantidad, @PrecioUnitario, @PorcentajeDescuento, @BaseImponible, @CodigoImpuesto, @TarifaImpuesto, @ValorImpuesto);
            """;

        using var conexion = _conexionFactory.CrearConexion();
        await AbrirConexionAsync(conexion, cancellationToken);
        using var transaccion = conexion.BeginTransaction();
        await conexion.ExecuteAsync(new CommandDefinition(sqlFactura, new
        {
            factura.Id,
            factura.NumeroDocumento,
            factura.RucEmisor,
            factura.RazonSocialEmisor,
            factura.CorreoCliente,
            factura.IdentificacionCliente,
            factura.NombreCliente,
            factura.Moneda,
            factura.Subtotal,
            factura.Impuestos,
            factura.Total,
            factura.ClaveAcceso,
            factura.XmlGenerado,
            Estado = factura.Estado.ToString(),
            factura.MensajeEstado,
            factura.FechaCreacion,
            factura.CertificadoId
        }, transaccion, cancellationToken: cancellationToken));

        foreach (var item in factura.Items)
        {
            await conexion.ExecuteAsync(new CommandDefinition(sqlItem, item, transaccion, cancellationToken: cancellationToken));
        }

        transaccion.Commit();
    }

    public async Task<Factura?> ObtenerPorIdAsync(Guid facturaId, CancellationToken cancellationToken)
    {
        const string sqlFactura = "SELECT * FROM Facturas WHERE Id = @facturaId";
        const string sqlItems = "SELECT * FROM FacturaItems WHERE FacturaId = @facturaId ORDER BY Descripcion";

        using var conexion = _conexionFactory.CrearConexion();
        await AbrirConexionAsync(conexion, cancellationToken);
        var dto = await conexion.QuerySingleOrDefaultAsync<FacturaDb>(new CommandDefinition(sqlFactura, new { facturaId }, cancellationToken: cancellationToken));
        if (dto is null)
        {
            return null;
        }

        var factura = dto.ToDomain();
        factura.Items = (await conexion.QueryAsync<FacturaItem>(new CommandDefinition(sqlItems, new { facturaId }, cancellationToken: cancellationToken))).ToList();
        return factura;
    }

    public async Task ActualizarAsync(Factura factura, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE Facturas
            SET Estado = @Estado,
                MensajeEstado = @MensajeEstado,
                XmlFirmado = @XmlFirmado,
                XmlAutorizado = @XmlAutorizado,
                FechaAutorizacion = @FechaAutorizacion
            WHERE Id = @Id;
            """;

        using var conexion = _conexionFactory.CrearConexion();
        await conexion.ExecuteAsync(new CommandDefinition(sql, new
        {
            factura.Id,
            Estado = factura.Estado.ToString(),
            factura.MensajeEstado,
            factura.XmlFirmado,
            factura.XmlAutorizado,
            factura.FechaAutorizacion
        }, cancellationToken: cancellationToken));
    }

    private static async Task AbrirConexionAsync(System.Data.IDbConnection conexion, CancellationToken cancellationToken)
    {
        if (conexion is System.Data.Common.DbConnection dbConnection)
        {
            await dbConnection.OpenAsync(cancellationToken);
        }
        else
        {
            conexion.Open();
        }
    }

    private sealed class FacturaDb
    {
        public Guid Id { get; init; }
        public string NumeroDocumento { get; init; } = string.Empty;
        public string RucEmisor { get; init; } = string.Empty;
        public string RazonSocialEmisor { get; init; } = string.Empty;
        public string CorreoCliente { get; init; } = string.Empty;
        public string IdentificacionCliente { get; init; } = string.Empty;
        public string NombreCliente { get; init; } = string.Empty;
        public string Moneda { get; init; } = string.Empty;
        public decimal Subtotal { get; init; }
        public decimal Impuestos { get; init; }
        public decimal Total { get; init; }
        public string ClaveAcceso { get; init; } = string.Empty;
        public string XmlGenerado { get; init; } = string.Empty;
        public string? XmlFirmado { get; init; }
        public string? XmlAutorizado { get; init; }
        public string Estado { get; init; } = string.Empty;
        public string MensajeEstado { get; init; } = string.Empty;
        public DateTimeOffset FechaCreacion { get; init; }
        public DateTimeOffset? FechaAutorizacion { get; init; }
        public Guid CertificadoId { get; init; }

        public Factura ToDomain() => new()
        {
            Id = Id,
            NumeroDocumento = NumeroDocumento,
            RucEmisor = RucEmisor,
            RazonSocialEmisor = RazonSocialEmisor,
            CorreoCliente = CorreoCliente,
            IdentificacionCliente = IdentificacionCliente,
            NombreCliente = NombreCliente,
            Moneda = Moneda,
            Subtotal = Subtotal,
            Impuestos = Impuestos,
            Total = Total,
            ClaveAcceso = ClaveAcceso,
            XmlGenerado = XmlGenerado,
            XmlFirmado = XmlFirmado,
            XmlAutorizado = XmlAutorizado,
            Estado = Enum.TryParse<EstadoFactura>(Estado, true, out var estado) ? estado : EstadoFactura.Pendiente,
            MensajeEstado = MensajeEstado,
            FechaCreacion = FechaCreacion,
            FechaAutorizacion = FechaAutorizacion,
            CertificadoId = CertificadoId
        };
    }
}
