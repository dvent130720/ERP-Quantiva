using Core.Web.Api.Empresas.Application.Abstractions;
using Core.Web.Api.Empresas.Application.DTOs;
using Core.Web.Api.Empresas.Domain.Entities;
using Core.Web.Api.Empresas.Domain.Exceptions;

namespace Core.Web.Api.Empresas.Application.Services;

public sealed class CompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IReadOnlyCollection<CompanyResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.GetAllAsync(cancellationToken);
        return companies.Select(Map).ToArray();
    }

    public async Task<CompanyResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No existe una empresa con id {id}.");

        return Map(company);
    }

    public async Task<CompanyResponse> CreateAsync(CompanyRequest request, CancellationToken cancellationToken)
    {
        Validate(request);

        var duplicated = await _companyRepository.GetByTaxIdAsync(request.TaxId.Trim(), cancellationToken);
        if (duplicated is not null)
        {
            throw new DomainValidationException($"Ya existe una empresa con RUC/identificación {request.TaxId}.");
        }

        var now = DateTime.UtcNow;
        var company = new Company
        {
            Id = Guid.NewGuid(),
            TaxId = request.TaxId.Trim(),
            LegalName = request.LegalName.Trim(),
            TradeName = request.TradeName?.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone?.Trim(),
            Address = request.Address?.Trim(),
            IsActive = request.IsActive,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var created = await _companyRepository.CreateAsync(company, cancellationToken);
        return Map(created);
    }

    public async Task<CompanyResponse> UpdateAsync(Guid id, CompanyRequest request, CancellationToken cancellationToken)
    {
        Validate(request);

        var current = await _companyRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"No existe una empresa con id {id}.");

        var duplicated = await _companyRepository.GetByTaxIdAsync(request.TaxId.Trim(), cancellationToken);
        if (duplicated is not null && duplicated.Id != id)
        {
            throw new DomainValidationException($"Ya existe una empresa con RUC/identificación {request.TaxId}.");
        }

        current.TaxId = request.TaxId.Trim();
        current.LegalName = request.LegalName.Trim();
        current.TradeName = request.TradeName?.Trim();
        current.Email = request.Email.Trim();
        current.Phone = request.Phone?.Trim();
        current.Address = request.Address?.Trim();
        current.IsActive = request.IsActive;
        current.UpdatedAtUtc = DateTime.UtcNow;

        var updated = await _companyRepository.UpdateAsync(current, cancellationToken);
        return Map(updated);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _companyRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new KeyNotFoundException($"No existe una empresa con id {id}.");
        }
    }

    private static CompanyResponse Map(Company company) => new(
        company.Id,
        company.TaxId,
        company.LegalName,
        company.TradeName,
        company.Email,
        company.Phone,
        company.Address,
        company.IsActive,
        company.CreatedAtUtc,
        company.UpdatedAtUtc);

    private static void Validate(CompanyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TaxId))
        {
            throw new DomainValidationException("El RUC/identificación es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.LegalName))
        {
            throw new DomainValidationException("La razón social es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            throw new DomainValidationException("El correo electrónico es inválido.");
        }
    }
}
