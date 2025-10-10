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
