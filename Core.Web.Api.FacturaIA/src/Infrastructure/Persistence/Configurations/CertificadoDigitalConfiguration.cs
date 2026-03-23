using Core.Web.Api.FacturaIA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Web.Api.FacturaIA.Infrastructure.Persistence.Configurations;

public class CertificadoDigitalConfiguration : IEntityTypeConfiguration<CertificadoDigital>
{
    public void Configure(EntityTypeBuilder<CertificadoDigital> builder)
    {
        builder.ToTable("certificados_digitales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.NombreArchivo).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Password).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Proveedor).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Archivo).IsRequired();
    }
}
