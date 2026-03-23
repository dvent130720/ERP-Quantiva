using Core.Web.Api.FacturaIA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Web.Api.FacturaIA.Infrastructure.Persistence.Configurations;

public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.ToTable("auditorias");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Accion).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Usuario).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Endpoint).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Data).HasColumnType("jsonb");
    }
}
