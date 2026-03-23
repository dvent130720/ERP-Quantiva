using Core.Web.Api.FacturaIA.Application.Interfaces;
using Core.Web.Api.FacturaIA.Application.Services;

namespace Core.Web.Api.FacturaIA.UnitTests;

public class AiServiceTests
{
    [Fact]
    public async Task ProcesarPromptAsync_ShouldReturnVentasIntent_WhenPromptContainsVentas()
    {
        var ventaRepository = new FakeVentaRepository();
        var ledgerRepository = new FakeLedgerRepository();
        var unitOfWork = new FakeUnitOfWork();
        var ventasService = new VentasService(ventaRepository, ledgerRepository, unitOfWork);
        var service = new AiService(ventasService, new FakeMetrics());

        var response = await service.ProcesarPromptAsync("¿Cuánto vendí este mes?", Guid.NewGuid());

        Assert.Equal("ventas_total", response.Intent);
    }

    private sealed class FakeMetrics : IAppMetrics
    {
        public void RecordAiIntent(string intent) { }
        public void RecordSriSubmission(string status) { }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }

    private sealed class FakeVentaRepository : IVentaRepository
    {
        public Task<Core.Web.Api.FacturaIA.Domain.Entities.Venta> AddAsync(Core.Web.Api.FacturaIA.Domain.Entities.Venta venta, CancellationToken cancellationToken = default) => Task.FromResult(venta);
        public Task<Core.Web.Api.FacturaIA.Domain.Entities.Venta?> GetByIdAsync(Guid ventaId, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult<Core.Web.Api.FacturaIA.Domain.Entities.Venta?>(null);
        public Task<decimal> ObtenerTotalAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult(150m);
    }

    private sealed class FakeLedgerRepository : ILedgerRepository
    {
        public Task<Core.Web.Api.FacturaIA.Domain.Entities.LedgerEntry> AddAsync(Core.Web.Api.FacturaIA.Domain.Entities.LedgerEntry entry, CancellationToken cancellationToken = default) => Task.FromResult(entry);
        public Task<IReadOnlyList<Core.Web.Api.FacturaIA.Domain.Entities.LedgerEntry>> GetByRangeAsync(DateTime inicio, DateTime fin, Guid tenantId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Core.Web.Api.FacturaIA.Domain.Entities.LedgerEntry>>(Array.Empty<Core.Web.Api.FacturaIA.Domain.Entities.LedgerEntry>());
    }
}
