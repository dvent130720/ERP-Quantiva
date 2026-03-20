using SriFacturacion.Application.DTOs;

namespace SriFacturacion.Application.Abstracciones.Facturacion;

public interface IServicioCertificados
{
    Task<SubirCertificadoResponse> SubirAsync(SubirCertificadoRequest request, CancellationToken cancellationToken);
}
