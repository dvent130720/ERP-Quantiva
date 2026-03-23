using Core.Web.Api.FacturaIA.Application.DTOs;
using FluentValidation;

namespace Core.Web.Api.FacturaIA.Application.Validators;

public class VentasRequestDtoValidator : AbstractValidator<VentasRequestDto>
{
    public VentasRequestDtoValidator()
    {
        RuleFor(x => x.FechaInicio).LessThanOrEqualTo(x => x.FechaFin);
    }
}
