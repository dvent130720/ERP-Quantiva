using Microsoft.Extensions.Options;
using SriFacturacion.Application.Abstracciones.Documentos;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Servicios.Archivos;

public sealed class RepositorioArchivosFacturaLocal : IRepositorioArchivosFactura
{
    private readonly string _directorio;

    public RepositorioArchivosFacturaLocal(IOptions<SeguridadOptions> options)
    {
        _directorio = options.Value.DirectorioAutorizados;
        Directory.CreateDirectory(_directorio);
    }

    public async Task<string> GuardarXmlAutorizadoAsync(Guid facturaId, string xml, CancellationToken cancellationToken)
    {
        var ruta = Path.Combine(_directorio, $"{facturaId:N}.xml");
        await File.WriteAllTextAsync(ruta, xml, cancellationToken);
        return ruta;
    }

    public Task<byte[]> ObtenerXmlAutorizadoAsync(Guid facturaId, CancellationToken cancellationToken)
    {
        var ruta = Path.Combine(_directorio, $"{facturaId:N}.xml");
        return File.ReadAllBytesAsync(ruta, cancellationToken);
    }
}
