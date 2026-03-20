using FluentValidation;
using SriFacturacion.Application.Abstracciones.Certificados;
using SriFacturacion.Application.Abstracciones.Facturacion;
using SriFacturacion.Application.Abstracciones.Infraestructura;
using SriFacturacion.Application.Abstracciones.Persistencia;
using SriFacturacion.Application.DTOs;
using SriFacturacion.Domain.Entidades;

namespace SriFacturacion.Application.Certificados;

public sealed class ServicioCertificados : IServicioCertificados
{
    private readonly IValidator<SubirCertificadoRequest> _validator;
    private readonly IValidadorCertificado _validadorCertificado;
    private readonly IAlmacenCertificados _almacenCertificados;
    private readonly ICertificadoRepository _certificadoRepository;
    private readonly ICifrador _cifrador;

    public ServicioCertificados(
        IValidator<SubirCertificadoRequest> validator,
        IValidadorCertificado validadorCertificado,
        IAlmacenCertificados almacenCertificados,
        ICertificadoRepository certificadoRepository,
        ICifrador cifrador)
    {
        _validator = validator;
        _validadorCertificado = validadorCertificado;
        _almacenCertificados = almacenCertificados;
        _certificadoRepository = certificadoRepository;
        _cifrador = cifrador;
    }

    public async Task<SubirCertificadoResponse> SubirAsync(SubirCertificadoRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var resultado = await _validadorCertificado.ValidarAsync(request, cancellationToken);
        if (!resultado.Exitoso || resultado.Datos is null)
        {
            throw new InvalidOperationException(resultado.Mensaje);
        }

        var ruta = await _almacenCertificados.GuardarAsync(request.NombreArchivo, request.Archivo, cancellationToken);
        var certificado = new Certificado
        {
            Id = resultado.Datos.CertificadoId,
            NombreArchivo = request.NombreArchivo,
            RutaArchivoSeguro = ruta,
            ClaveCifrada = _cifrador.Cifrar(request.Clave),
            Thumbprint = resultado.Datos.Thumbprint,
            RucTitular = request.RucTitular,
            FechaExpiracion = resultado.Datos.FechaExpiracion,
            Activo = true
        };

        await _certificadoRepository.CrearAsync(certificado, cancellationToken);
        return resultado.Datos;
    }
}
