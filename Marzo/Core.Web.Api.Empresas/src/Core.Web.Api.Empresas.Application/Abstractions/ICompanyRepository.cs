using Core.Web.Api.Empresas.Domain.Entities;

namespace Core.Web.Api.Empresas.Application.Abstractions;

public interface ICompanyRepository
{
    Task<IReadOnlyCollection<Company>> GetAllAsync(CancellationToken cancellationToken);
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Company?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken);
    Task<Company> CreateAsync(Company company, CancellationToken cancellationToken);
    Task<Company> UpdateAsync(Company company, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
