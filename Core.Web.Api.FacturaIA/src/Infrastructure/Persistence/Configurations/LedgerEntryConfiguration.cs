using Core.Web.Api.FacturaIA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Web.Api.FacturaIA.Infrastructure.Persistence.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("ledger_entries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Tipo).HasMaxLength(32).IsRequired();
        builder.Property(x => x.CuentaDebito).HasMaxLength(64).IsRequired();
        builder.Property(x => x.CuentaCredito).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Monto).HasPrecision(18, 2);
        builder.Property(x => x.Moneda).HasMaxLength(8).IsRequired();
        builder.HasOne(x => x.Venta)
            .WithMany(x => x.LedgerEntries)
            .HasForeignKey(x => x.VentaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
