using Core.Web.Api.FacturaIA.Application.DTOs;
using FluentValidation;

namespace Core.Web.Api.FacturaIA.Application.Validators;

public class SriCertificateUploadRequestValidator : AbstractValidator<SriCertificateUploadRequest>
{
    public SriCertificateUploadRequestValidator()
    {
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Provider).NotEmpty().MaximumLength(32);
    }
}
