using Core.Web.Api.FacturaIA.Application.DTOs;
using FluentValidation;

namespace Core.Web.Api.FacturaIA.Application.Validators;

public class LedgerEntryCreateDtoValidator : AbstractValidator<LedgerEntryCreateDto>
{
    public LedgerEntryCreateDtoValidator()
    {
        RuleFor(x => x.Tipo).NotEmpty();
        RuleFor(x => x.CuentaDebito).NotEmpty();
        RuleFor(x => x.CuentaCredito).NotEmpty();
        RuleFor(x => x.Monto).GreaterThan(0);
    }
}
