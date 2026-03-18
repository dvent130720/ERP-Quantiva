# Quantiva Backend (.NET 8)

## Qué corregí respecto a la primera versión
Esta iteración fortalece el backend anterior en tres frentes:
- endurece validaciones y manejo de errores con respuestas `problem+json`
- amplía la administración de datos con operaciones `GET/POST/PUT/DELETE` para clientes y productos
- añade despliegue productivo con Docker, PostgreSQL y Nginx para Contabo VPS

## Arquitectura propuesta
- `Quantiva.Api`: capa HTTP, autenticación JWT, Swagger, middleware global de errores y resolución de tenant.
- `Quantiva.Infrastructure`: acceso a PostgreSQL con EF Core, hashing PBKDF2, generación de tokens, seed inicial y servicios de negocio.
- `Quantiva.Application`: contratos, DTOs, validaciones declarativas y excepciones de aplicación.
- `Quantiva.Domain`: entidades de dominio (`Tenant`, `AppUser`, `Customer`, `Product`, `AuditTrail`).

## Capacidades implementadas
- Multi-tenant por tenant lógico con filtros globales por `TenantId`.
- Resolución de tenant por header `X-Tenant-Slug`, dominio directo o subdominio (`tenant.quantiva-solutions.com`).
- Auditoría automática de altas, modificaciones y eliminaciones en `SaveChangesAsync`.
- JWT con roles `PlatformAdmin` y `TenantAdmin`.
- Endpoints CRUD para clientes y productos.
- Gestión de tenants desde plataforma.
- Middleware centralizado de errores con `problem+json`.
- Artefactos Docker para desplegar en VPS Linux con PostgreSQL.

## Endpoints base
- `POST /api/auth/login`
- `GET|POST /api/platform/tenants`
- `GET /api/platform/tenants/current`
- `GET|POST|PUT|DELETE /api/customers`
- `GET|POST|PUT|DELETE /api/products`
- `GET /api/audits`
- `GET /health`
- `GET /swagger`

## Credenciales semilla
> Cambiarlas inmediatamente después del primer despliegue.

- Platform admin: `platform@quantiva-solutions.com` / `ChangeMe123!`
- Tenant admin demo: `admin@quantiva-solutions.com` / `ChangeMe123!`
- Tenant demo: `quantiva-demo`
- Dominio demo sugerido: `demo.quantiva-solutions.com`

## Configuración principal
Variables importantes:
- `ConnectionStrings__DefaultConnection`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__SecretKey`
- `TenantResolution__SharedHosts`
- `TenantResolution__WildcardBaseDomains`

## Despliegue recomendado en Contabo con Docker
Como ya tienes Docker instalado en el VPS, esta es la ruta más simple y sólida.

### 1) Subir el proyecto al VPS
```bash
cd /opt
git clone <TU-REPO> quantiva
cd quantiva/backend
cp .env.example .env
```

### 2) Editar variables de entorno
Edita `.env` con secretos reales:
```bash
nano .env
```

Ejemplo:
```env
POSTGRES_DB=quantiva
POSTGRES_USER=quantiva
POSTGRES_PASSWORD=UnaClavePostgresMuySegura
JWT_ISSUER=quantiva-api
JWT_AUDIENCE=quantiva-clients
JWT_SECRET_KEY=CAMBIA_ESTA_LLAVE_SUPER_SEGURA_DE_MAS_DE_32_CARACTERES
ROOT_DOMAIN=quantiva-solutions.com
API_DOMAIN=api.quantiva-solutions.com
```

### 3) Levantar la API y PostgreSQL
```bash
docker compose up -d --build
```

### 4) Verificar contenedores y logs
```bash
docker compose ps
docker compose logs -f api
docker compose logs -f postgres
```

### 5) Configurar Nginx en el VPS
Copia `backend/deploy/nginx/quantiva-api.conf` a Nginx:
```bash
sudo cp /opt/quantiva/backend/deploy/nginx/quantiva-api.conf /etc/nginx/sites-available/quantiva-api.conf
sudo ln -s /etc/nginx/sites-available/quantiva-api.conf /etc/nginx/sites-enabled/quantiva-api.conf
sudo nginx -t
sudo systemctl reload nginx
```

### 6) Certificados SSL con Let's Encrypt
Si aún no tienes los certificados:
```bash
sudo apt update
sudo apt install -y certbot python3-certbot-nginx
sudo certbot --nginx -d quantiva-solutions.com -d api.quantiva-solutions.com -d '*.quantiva-solutions.com'
```

> Nota: algunos escenarios wildcard requieren validación DNS con el proveedor del dominio. Si no quieres wildcard al inicio, publica primero `quantiva-solutions.com` y `api.quantiva-solutions.com`.

### 7) Probar salud del backend
```bash
curl http://127.0.0.1:8080/health
curl https://api.quantiva-solutions.com/health
```

## Flujo multi-tenant sugerido en producción
- Usa `api.quantiva-solutions.com` como host principal del backend.
- Para clientes multi-tenant puedes:
  - enviar `X-Tenant-Slug: quantiva-demo`, o
  - entrar por subdominio como `quantiva-demo.quantiva-solutions.com`
- Mantén `quantiva-solutions.com` y `api.quantiva-solutions.com` como hosts compartidos, no ligados automáticamente a un tenant.

## Operación diaria
### Actualizar a una nueva versión
```bash
cd /opt/quantiva
git pull
cd backend
docker compose up -d --build
```

### Reiniciar servicios
```bash
cd /opt/quantiva/backend
docker compose restart api
docker compose restart postgres
```

### Backup manual de PostgreSQL
```bash
docker exec -t quantiva-postgres pg_dump -U quantiva -d quantiva > backup_quantiva.sql
```

## Siguientes pasos recomendados
- Integrar el frontend Angular actual contra esta API real.
- Sustituir `EnsureCreated` por migraciones EF Core versionadas cuando el entorno de build tenga `dotnet` disponible.
- Añadir refresh tokens, observabilidad y tests automáticos de integración.
