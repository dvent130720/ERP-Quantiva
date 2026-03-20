using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Abstracciones.Persistencia;

public interface ICertificadoRepository
{
    Task CrearAsync(Certificado certificado, CancellationToken cancellationToken);
    Task<Certificado?> ObtenerPorIdAsync(Guid certificadoId, CancellationToken cancellationToken);
}
