# Core.Web.Api.Empresas

API REST en .NET 8 para administrar empresas con PostgreSQL usando DDL, principios SOLID, autenticación JWT, observabilidad y manejo centralizado de errores.

## Qué incluye

- CRUD de empresas (`/api/companies`).
- Login con JWT (`/api/auth/login`) y registro estructurado hacia Loki/Grafana mediante Serilog.
- Inicialización de base de datos con script DDL (`scripts/init-ddl.sql`).
- Endpoint de salud (`/health`).
- Endpoint Prometheus (`/metrics`).
- Middleware global de errores con `ProblemDetails`.

## Estructura

```text
Marzo/Core.Web.Api.Empresas/
├── scripts/init-ddl.sql
├── src/
│   ├── Core.Web.Api.Empresas.Api/
│   ├── Core.Web.Api.Empresas.Application/
│   ├── Core.Web.Api.Empresas.Domain/
│   └── Core.Web.Api.Empresas.Infrastructure/
└── Core.Web.Api.Empresas.sln
```

## Variables principales

- `ConnectionStrings__Postgres`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__Secret`
- `Jwt__AccessTokenMinutes`
- `Loki__Url`
- `Loki__Username`
- `Loki__Password`

## Uso esperado

```bash
dotnet restore Marzo/Core.Web.Api.Empresas/Core.Web.Api.Empresas.sln
dotnet run --project Marzo/Core.Web.Api.Empresas/src/Core.Web.Api.Empresas.Api
```


## Usuario inicial

- Usuario: `admin`
- Clave: `Admin123!`
