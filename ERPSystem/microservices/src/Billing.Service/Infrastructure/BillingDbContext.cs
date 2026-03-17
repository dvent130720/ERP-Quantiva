using Billing.Service.Domain;
using Microsoft.EntityFrameworkCore;

namespace Billing.Service.Infrastructure;

public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
{
    public DbSet<Comprobante> Comprobantes => Set<Comprobante>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comprobante>(entity =>
        {
            entity.ToTable("comprobantes");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.TenantId, x.ClaveAcceso }).IsUnique();
        });
    }
}
