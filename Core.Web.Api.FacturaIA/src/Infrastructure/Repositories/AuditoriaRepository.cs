using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Domain.Entities;
using Core.Web.Api.FacturaIA.Infrastructure.Persistence;

namespace Core.Web.Api.FacturaIA.Infrastructure.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly AppDbContext _context;

    public AuditoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Auditoria auditoria, CancellationToken cancellationToken = default)
    {
        await _context.Auditorias.AddAsync(auditoria, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
