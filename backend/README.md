# Quantiva Backend (.NET 8)

## Diagnóstico del repositorio actual
El repositorio original solo contenía un frontend Angular enfocado en autenticación mock. No existía una API real, persistencia, multi-tenancy, seguridad por JWT ni trazabilidad de auditoría. Por eso agregué una solución backend desacoplada en .NET 8 lista para correr en Linux sobre Contabo VPS.

## Arquitectura propuesta
- `Quantiva.Api`: capa HTTP, autenticación JWT, Swagger, health checks y resolución de tenant por `X-Tenant-Slug` o dominio.
- `Quantiva.Infrastructure`: acceso a PostgreSQL con EF Core, hashing PBKDF2, generación de tokens, seed inicial y servicios de aplicación.
- `Quantiva.Application`: contratos, DTOs y abstracciones.
- `Quantiva.Domain`: entidades de dominio (`Tenant`, `AppUser`, `Customer`, `Product`, `AuditTrail`).

## Capacidades implementadas
- Multi-tenant por tenant lógico con filtro global en entidades de negocio.
- Resolución de tenant por header `X-Tenant-Slug` o dominio configurado.
- Auditoría automática de altas, modificaciones y eliminaciones.
- JWT con roles `PlatformAdmin` y `TenantAdmin`.
- Gestión inicial de clientes, productos, tenants y consulta de auditorías.
- Seed de datos para ambiente inicial.

## Endpoints base
- `POST /api/auth/login`
- `GET|POST /api/platform/tenants`
- `GET /api/platform/tenants/current`
- `GET|POST|PUT /api/customers`
- `GET|POST /api/products`
- `GET /api/audits`
- `GET /health`
- `GET /swagger`

## Credenciales semilla
> Cambiarlas inmediatamente después del primer despliegue.

- Platform admin: `platform@quantiva-solutions.com` / `ChangeMe123!`
- Tenant admin demo: `admin@quantiva-solutions.com` / `ChangeMe123!`
- Tenant demo: `quantiva-demo`

## Variables y configuración recomendada para Contabo
1. Instala `.NET 8 SDK` y `PostgreSQL 16` o usa un PostgreSQL administrado.
2. Crea la base de datos `quantiva`.
3. Ajusta `src/Quantiva.Api/appsettings.Production.json` o variables de entorno:
   - `ConnectionStrings__DefaultConnection`
   - `Jwt__Issuer`
   - `Jwt__Audience`
   - `Jwt__SecretKey`
4. Configura Nginx como reverse proxy hacia Kestrel.
5. Crea un servicio `systemd` para publicar el backend.

## Publicación sugerida en VPS Linux
```bash
cd backend/src/Quantiva.Api
dotnet restore
dotnet publish -c Release -o /var/www/quantiva-api
```

Ejemplo de servicio `systemd` (`/etc/systemd/system/quantiva-api.service`):
```ini
[Unit]
Description=Quantiva API
After=network.target

[Service]
WorkingDirectory=/var/www/quantiva-api
ExecStart=/usr/bin/dotnet /var/www/quantiva-api/Quantiva.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=quantiva-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:8080
Environment=ConnectionStrings__DefaultConnection=Host=127.0.0.1;Port=5432;Database=quantiva;Username=quantiva;Password=super-segura
Environment=Jwt__Issuer=quantiva-api
Environment=Jwt__Audience=quantiva-clients
Environment=Jwt__SecretKey=CAMBIA_ESTA_LLAVE_SUPER_SEGURA_DE_MAS_DE_32_CARACTERES

[Install]
WantedBy=multi-user.target
```

Ejemplo de Nginx para `https://quantiva-solutions.com`:
```nginx
server {
    server_name quantiva-solutions.com api.quantiva-solutions.com;

    location / {
        proxy_pass         http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }

    listen 443 ssl http2;
    ssl_certificate /etc/letsencrypt/live/quantiva-solutions.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/quantiva-solutions.com/privkey.pem;
}
```

## Flujo operativo multi-tenant
- El frontend o cliente envía `X-Tenant-Slug: quantiva-demo`.
- El middleware resuelve el tenant antes de ejecutar controladores.
- EF Core aplica filtros globales a `customers` y `products` por `TenantId`.
- `SaveChangesAsync` registra la huella de auditoría con usuario/IP/cambios JSON.

## Siguientes pasos recomendados
- Integrar esta API con el frontend Angular actual sustituyendo el `AuthService` mock.
- Añadir migrations versionadas y pipeline CI/CD cuando el entorno tenga .NET SDK disponible.
- Incorporar refresh tokens, rotación de secretos y observabilidad (Serilog + OpenTelemetry).
