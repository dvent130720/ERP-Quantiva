using FluentValidation;
using SriFacturacion.Application.DTOs;

namespace SriFacturacion.Application.Certificados;

public sealed class SubirCertificadoRequestValidator : AbstractValidator<SubirCertificadoRequest>
{
    public SubirCertificadoRequestValidator()
    {
        RuleFor(x => x.NombreArchivo).NotEmpty().Must(x => x.EndsWith(".p12", StringComparison.OrdinalIgnoreCase)).WithMessage("El archivo debe ser .p12");
        RuleFor(x => x.Archivo).NotEmpty();
        RuleFor(x => x.Clave).NotEmpty();
        RuleFor(x => x.RucTitular).Length(13);
    }
}
