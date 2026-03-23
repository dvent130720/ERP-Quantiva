using Core.Web.Api.FacturaIA.Application.DTOs;
using FluentValidation;

namespace Core.Web.Api.FacturaIA.Application.Validators;

public class VentaCreateDtoValidator : AbstractValidator<VentaCreateDto>
{
    public VentaCreateDtoValidator()
    {
        RuleFor(x => x.Total).GreaterThan(0);
        RuleFor(x => x.NumeroComprobante).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Estado).NotEmpty().MaximumLength(32);
    }
}
