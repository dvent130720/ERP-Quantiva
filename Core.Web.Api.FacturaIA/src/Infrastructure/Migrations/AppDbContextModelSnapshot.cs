using System;
using Core.Web.Api.FacturaIA.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Core.Web.Api.FacturaIA.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.10")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("Core.Web.Api.FacturaIA.Domain.Entities.Auditoria", b =>
        {
            b.Property<Guid>("Id");
            b.Property<string>("Accion").HasMaxLength(128);
            b.Property<DateTime>("CreatedAt");
            b.Property<string>("Data").HasColumnType("jsonb");
            b.Property<string>("Endpoint").HasMaxLength(256);
            b.Property<DateTime>("Fecha");
            b.Property<int>("StatusCode");
            b.Property<Guid>("TenantId");
            b.Property<string>("Usuario").HasMaxLength(256);
            b.HasKey("Id");
            b.HasIndex("TenantId");
            b.ToTable("auditorias");
        });

        modelBuilder.Entity("Core.Web.Api.FacturaIA.Domain.Entities.CertificadoDigital", b =>
        {
            b.Property<Guid>("Id");
            b.Property<byte[]>("Archivo");
            b.Property<DateTime>("CreatedAt");
            b.Property<DateTime?>("ExpiraEn");
            b.Property<string>("NombreArchivo").HasMaxLength(256);
            b.Property<string>("Password").HasMaxLength(256);
            b.Property<string>("Proveedor").HasMaxLength(32);
            b.Property<Guid>("TenantId");
            b.HasKey("Id");
            b.HasIndex("TenantId");
            b.ToTable("certificados_digitales");
        });

        modelBuilder.Entity("Core.Web.Api.FacturaIA.Domain.Entities.LedgerEntry", b =>
        {
            b.Property<Guid>("Id");
            b.Property<string>("CuentaCredito").HasMaxLength(64);
            b.Property<string>("CuentaDebito").HasMaxLength(64);
            b.Property<DateTime>("CreatedAt");
            b.Property<DateTime>("Fecha");
            b.Property<decimal>("Monto").HasPrecision(18, 2);
            b.Property<string>("Moneda").HasMaxLength(8);
            b.Property<Guid>("TenantId");
            b.Property<string>("Tipo").HasMaxLength(32);
            b.Property<Guid?>("VentaId");
            b.HasKey("Id");
            b.HasIndex("TenantId");
            b.HasIndex("VentaId");
            b.ToTable("ledger_entries");
        });

        modelBuilder.Entity("Core.Web.Api.FacturaIA.Domain.Entities.Tenant", b =>
        {
            b.Property<Guid>("Id");
            b.Property<DateTime>("CreatedAt");
            b.Property<string>("Nombre").HasMaxLength(128);
            b.Property<string>("PaisCodigo").HasMaxLength(8);
            b.Property<Guid>("TenantId");
            b.Property<string>("TimeZone").HasMaxLength(64);
            b.HasKey("Id");
            b.HasIndex("TenantId");
            b.ToTable("tenants");
        });

        modelBuilder.Entity("Core.Web.Api.FacturaIA.Domain.Entities.Venta", b =>
        {
            b.Property<Guid>("Id");
            b.Property<DateTime>("CreatedAt");
            b.Property<string>("Estado").HasMaxLength(32);
            b.Property<DateTime>("Fecha");
            b.Property<string>("Moneda").HasMaxLength(8);
            b.Property<string>("NumeroComprobante").HasMaxLength(64);
            b.Property<Guid>("TenantId");
            b.Property<decimal>("Total").HasPrecision(18, 2);
            b.HasKey("Id");
            b.HasIndex("TenantId");
            b.ToTable("ventas");
        });

        modelBuilder.Entity("Core.Web.Api.FacturaIA.Domain.Entities.LedgerEntry", b =>
        {
            b.HasOne("Core.Web.Api.FacturaIA.Domain.Entities.Venta", "Venta")
                .WithMany("LedgerEntries")
                .HasForeignKey("VentaId")
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
