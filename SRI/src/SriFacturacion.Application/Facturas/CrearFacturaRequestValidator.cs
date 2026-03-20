using FluentValidation;
using SriFacturacion.Application.DTOs;

namespace SriFacturacion.Application.Facturas;

public sealed class CrearFacturaRequestValidator : AbstractValidator<CrearFacturaRequest>
{
    public CrearFacturaRequestValidator()
    {
        RuleFor(x => x.NumeroDocumento).NotEmpty().MaximumLength(17);
        RuleFor(x => x.RucEmisor).Length(13);
        RuleFor(x => x.RazonSocialEmisor).NotEmpty().MaximumLength(300);
        RuleFor(x => x.CorreoCliente).NotEmpty().EmailAddress();
        RuleFor(x => x.IdentificacionCliente).NotEmpty().MaximumLength(13);
        RuleFor(x => x.NombreCliente).NotEmpty().MaximumLength(300);
        RuleFor(x => x.CertificadoId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.CodigoPrincipal).NotEmpty();
            item.RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(300);
            item.RuleFor(x => x.Cantidad).GreaterThan(0);
            item.RuleFor(x => x.PrecioUnitario).GreaterThan(0);
            item.RuleFor(x => x.PorcentajeDescuento).GreaterThanOrEqualTo(0);
            item.RuleFor(x => x.CodigoImpuesto).NotEmpty();
        });
    }
}
