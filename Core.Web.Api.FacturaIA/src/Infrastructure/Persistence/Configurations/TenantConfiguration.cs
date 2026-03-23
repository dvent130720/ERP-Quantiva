using Core.Web.Api.FacturaIA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Web.Api.FacturaIA.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre).HasMaxLength(128).IsRequired();
        builder.Property(x => x.PaisCodigo).HasMaxLength(8).IsRequired();
        builder.Property(x => x.TimeZone).HasMaxLength(64).IsRequired();
    }
}
