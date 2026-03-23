using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Web.Api.FacturaIA.Infrastructure.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "auditorias",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Accion = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Usuario = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                Data = table.Column<string>(type: "jsonb", nullable: false),
                Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Endpoint = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                StatusCode = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_auditorias", x => x.Id));

        migrationBuilder.CreateTable(
            name: "certificados_digitales",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                NombreArchivo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                Archivo = table.Column<byte[]>(type: "bytea", nullable: false),
                Password = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                Proveedor = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                ExpiraEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_certificados_digitales", x => x.Id));

        migrationBuilder.CreateTable(
            name: "tenants",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Nombre = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                PaisCodigo = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                TimeZone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_tenants", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ventas",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Estado = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                NumeroComprobante = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Moneda = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ventas", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ledger_entries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Tipo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                CuentaDebito = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                CuentaCredito = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Moneda = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                VentaId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ledger_entries", x => x.Id);
                table.ForeignKey(
                    name: "FK_ledger_entries_ventas_VentaId",
                    column: x => x.VentaId,
                    principalTable: "ventas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex(name: "IX_auditorias_TenantId", table: "auditorias", column: "TenantId");
        migrationBuilder.CreateIndex(name: "IX_certificados_digitales_TenantId", table: "certificados_digitales", column: "TenantId");
        migrationBuilder.CreateIndex(name: "IX_tenants_TenantId", table: "tenants", column: "TenantId");
        migrationBuilder.CreateIndex(name: "IX_ventas_TenantId", table: "ventas", column: "TenantId");
        migrationBuilder.CreateIndex(name: "IX_ledger_entries_TenantId", table: "ledger_entries", column: "TenantId");
        migrationBuilder.CreateIndex(name: "IX_ledger_entries_VentaId", table: "ledger_entries", column: "VentaId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "auditorias");
        migrationBuilder.DropTable(name: "certificados_digitales");
        migrationBuilder.DropTable(name: "ledger_entries");
        migrationBuilder.DropTable(name: "tenants");
        migrationBuilder.DropTable(name: "ventas");
    }
}
