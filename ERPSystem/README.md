# 🏢 ERP System - Sistema de Gestión Empresarial SaaS

Sistema ERP completo con integración al SRI (Ecuador)

## 🏗️ Arquitectura

- **Clean Architecture**
- **.NET 8**
- **PostgreSQL 16**
- **Angular 18**

## 📁 Estructura del Proyecto

```
ERPSystem/
├── src/
│   ├── ERPSystem.Domain/          # Entidades, Value Objects, Interfaces
│   ├── ERPSystem.Application/     # Casos de uso, CQRS, DTOs
│   ├── ERPSystem.Infrastructure/  # EF Core, Servicios externos
│   ├── ERPSystem.API/             # Endpoints, Middleware
│   └── ERPSystem.Shared/          # Utilidades compartidas
└── tests/
    ├── ERPSystem.UnitTests/
    └── ERPSystem.IntegrationTests/
```

## 🚀 Comandos

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar API
dotnet run --project src/ERPSystem.API

# Ejecutar tests
dotnet test
```

## 📦 Tecnologías

- .NET 8
- Entity Framework Core 8
- PostgreSQL 16
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- JWT Authentication
- Serilog
- Hangfire
- Redis
- Carter (Minimal APIs)

## 🎯 Integración SRI

- Facturación Electrónica
- Firma Digital XML
- Comunicación SOAP
- Validación automática

## 🧩 Módulos implementados

### Frontend Angular minimalista (`/frontend`)
- Dashboard de **Productos** con total y estado de stock (`OK`, `BAJO`, `SIN_STOCK`).
- Vista de **Ventas por calendario** usando `Angular Material Calendar`.
- Sección de **Facturación electrónica** con integración API al endpoint de recepción.
- Sección de **Contabilidad** con accesos a reportes ATS, IVA mensual y ATS simplificado.

### API ERP (Minimal APIs)
- `GET /api/dashboard/products-stock` → resumen visual de productos y stock.
- `GET /api/dashboard/sales-history` → historial de ventas por rango de fechas (ideal para calendario).
- `POST /api/electronic-billing/receive-invoice` → API para recibir factura, enviar a SRI y encolar post-proceso.
- `GET /api/accounting/ats` → reporte ATS.
- `GET /api/accounting/iva-monthly` → IVA mensual.
- `GET /api/accounting/ats-simplified` → ATS simplificado.

### Worker service (background)
- Cola interna para post-proceso de facturas.
- Genera archivo PDF simplificado de factura.
- Simula envío de WhatsApp (logging) para confirmación de entrega.

## 🐘 PostgreSQL
Se incluye `docker-compose.yml` para levantar Postgres 16:

```bash
docker compose up -d
```

Cadena ejemplo:

`Host=localhost;Port=5432;Database=erpdb;Username=erp;Password=erp123`
