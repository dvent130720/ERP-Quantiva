using Core.Web.Api.FacturaIA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Web.Api.FacturaIA.Infrastructure.Persistence.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("ventas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Total).HasPrecision(18, 2);
        builder.Property(x => x.Estado).HasMaxLength(32).IsRequired();
        builder.Property(x => x.NumeroComprobante).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Moneda).HasMaxLength(8).IsRequired();
    }
}
