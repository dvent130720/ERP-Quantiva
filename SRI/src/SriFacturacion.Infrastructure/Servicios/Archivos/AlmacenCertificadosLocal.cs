using Microsoft.Extensions.Options;
using SriFacturacion.Application.Abstracciones.Certificados;
using SriFacturacion.Infrastructure.Opciones;

namespace SriFacturacion.Infrastructure.Servicios.Archivos;

public sealed class AlmacenCertificadosLocal : IAlmacenCertificados
{
    private readonly SeguridadOptions _options;

    public AlmacenCertificadosLocal(IOptions<SeguridadOptions> options)
    {
        _options = options.Value;
        Directory.CreateDirectory(_options.DirectorioCertificados);
    }

    public async Task<string> GuardarAsync(string nombreArchivo, byte[] contenido, CancellationToken cancellationToken)
    {
        var ruta = Path.Combine(_options.DirectorioCertificados, $"{Guid.NewGuid():N}_{Path.GetFileName(nombreArchivo)}");
        await File.WriteAllBytesAsync(ruta, contenido, cancellationToken);
        return ruta;
    }

    public Task<byte[]> ObtenerAsync(string ruta, CancellationToken cancellationToken)
        => File.ReadAllBytesAsync(ruta, cancellationToken);
}
