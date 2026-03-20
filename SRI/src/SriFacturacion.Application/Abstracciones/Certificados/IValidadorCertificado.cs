using SriFacturacion.Application.DTOs;
using SriFacturacion.Domain.ObjetosValor;

namespace SriFacturacion.Application.Abstracciones.Certificados;

public interface IValidadorCertificado
{
    Task<ResultadoOperacion<SubirCertificadoResponse>> ValidarAsync(SubirCertificadoRequest request, CancellationToken cancellationToken);
}
