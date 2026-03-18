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

### 0) Si ya tienes Nginx + Certbot funcionando en el VPS, valida esto primero
Antes de tocar nada, revisa qué configuración activa tienes hoy:
```bash
sudo nginx -t
sudo systemctl status nginx --no-pager
sudo rg -n "server_name|ssl_certificate|proxy_pass" /etc/nginx/sites-available /etc/nginx/sites-enabled
sudo certbot certificates
```

Si ya existe un bloque para `quantiva-solutions.com`, no hace falta reinstalar Certbot desde cero. Solo debes:
- confirmar que el certificado vigente cubre `quantiva-solutions.com` y `api.quantiva-solutions.com`
- apuntar el `proxy_pass` al backend Docker en `http://127.0.0.1:8080`
- recargar Nginx con `sudo systemctl reload nginx`

### Ruta real para tu VPS (según lo que me compartiste)
En tu servidor ya confirmaste que el repo quedó aquí:
```bash
/var/www/ERP-Quantiva
```

Y además hoy no tienes contenedores activos, porque `docker ps` devolvió vacío. Eso significa que estás en un buen punto para hacer el primer despliegue limpio desde esta rama.

### 1) Subir el proyecto al VPS
```bash
cd /var/www
git clone <TU-REPO> ERP-Quantiva
cd /var/www/ERP-Quantiva/backend
cp .env.example .env
```

### 2) Editar variables de entorno
Edita `.env` con secretos reales:
```bash
nano .env
```

Importante: **aunque hoy no tengas una base de datos instalada manualmente**, sí necesitas definir `POSTGRES_PASSWORD`.
No es la contraseña de una base ya existente: es la contraseña que Docker usará para **crear por primera vez** el contenedor de PostgreSQL, el usuario `POSTGRES_USER` y la base `POSTGRES_DB`.

En otras palabras:
- `docker compose` levantará un contenedor nuevo llamado `quantiva-postgres`
- ese contenedor inicializará PostgreSQL solo
- con `POSTGRES_USER`, `POSTGRES_PASSWORD` y `POSTGRES_DB` se crea el usuario y la base inicial
- la API luego se conecta a ese contenedor usando esos mismos datos

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

Si no tienes ninguna BD previa, está perfecto: este proyecto ya la crea dentro de Docker.
Solo recuerda que esa contraseña debe quedar guardada en tu `.env`, porque también la usa la cadena de conexión del backend.

### 3) Levantar la API y PostgreSQL
```bash
docker compose up -d --build
```

### 3.1) Qué pasa la primera vez que ejecutas Docker
En el primer arranque, Compose hará esto automáticamente:
1. descargará la imagen `postgres:16-alpine`
2. creará el volumen `quantiva_postgres_data`
3. inicializará PostgreSQL con el usuario, password y base del `.env`
4. levantará la API y la conectará al host `postgres` interno de Docker

Por eso sí necesitas inventar una contraseña inicial aunque antes no existiera ninguna base de datos en el VPS.

### 4) Verificar contenedores y logs
```bash
docker compose ps
docker compose logs -f api
docker compose logs -f postgres
```

### 5) Configurar o ajustar Nginx en el VPS
Si ya tienes un virtual host activo, edítalo y deja el `proxy_pass` así:
```nginx
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
```

Si prefieres instalar el archivo del repo completo:
```bash
sudo cp /var/www/ERP-Quantiva/backend/deploy/nginx/quantiva-api.conf /etc/nginx/sites-available/quantiva-api.conf
sudo ln -sf /etc/nginx/sites-available/quantiva-api.conf /etc/nginx/sites-enabled/quantiva-api.conf
sudo nginx -t
sudo systemctl reload nginx
```

### 6) Certificados SSL con Let's Encrypt
Si ya tienes certificados, revisa solo renovación y cobertura:
```bash
sudo certbot certificates
sudo systemctl status certbot.timer --no-pager
```

Si todavía no existe el certificado para `api.quantiva-solutions.com`, créalo así:
```bash
sudo certbot --nginx -d quantiva-solutions.com -d api.quantiva-solutions.com
```

> Para wildcard `*.quantiva-solutions.com` normalmente necesitas challenge DNS. Si hoy ya te funciona Nginx con HTTPS en el dominio raíz, lo más práctico es publicar primero el backend en `api.quantiva-solutions.com` y dejar wildcard para después.

### 7) Probar salud del backend
```bash
curl http://127.0.0.1:8080/health
curl -I https://api.quantiva-solutions.com/health
curl https://api.quantiva-solutions.com/health
```

### 8) Despliegue rápido con script
También dejé un script para repetir despliegues en el VPS:
```bash
cd /var/www/ERP-Quantiva/backend
bash deploy/scripts/contabo-deploy.sh
```

Este script hace `git pull`, `docker compose up -d --build`, valida `/health` local y recarga Nginx si encuentra la configuración del repo.

## Flujo multi-tenant sugerido en producción
- Usa `api.quantiva-solutions.com` como host principal del backend.
- Para clientes multi-tenant puedes:
  - enviar `X-Tenant-Slug: quantiva-demo`, o
  - entrar por subdominio como `quantiva-demo.quantiva-solutions.com`
- Mantén `quantiva-solutions.com` y `api.quantiva-solutions.com` como hosts compartidos, no ligados automáticamente a un tenant.

## Operación diaria
### Actualizar a una nueva versión
```bash
cd /var/www/ERP-Quantiva
git pull
cd backend
docker compose up -d --build
```

### Reiniciar servicios
```bash
cd /var/www/ERP-Quantiva/backend
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
