using Core.Web.Api.FacturaIA.Application.DTOs;
using FluentValidation;

namespace Core.Web.Api.FacturaIA.Application.Validators;

public class AiQueryRequestDtoValidator : AbstractValidator<AiQueryRequestDto>
{
    public AiQueryRequestDtoValidator()
    {
        RuleFor(x => x.Prompt).NotEmpty().MaximumLength(2048);
    }
}
