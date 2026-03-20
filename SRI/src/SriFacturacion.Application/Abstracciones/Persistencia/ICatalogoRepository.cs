using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Abstracciones.Persistencia;

public interface ICatalogoRepository
{
    Task<Catalogo?> ObtenerPorTipoCodigoAsync(string tipo, string codigo, CancellationToken cancellationToken);
}
