using Core.Web.Api.FacturaIA.Domain.Entities;

namespace Core.Web.Api.FacturaIA.Application.Interfaces;

public interface IAuditoriaRepository
{
    Task AddAsync(Auditoria auditoria, CancellationToken cancellationToken = default);
}
