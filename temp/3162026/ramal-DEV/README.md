# AuthBackend (.NET 8 Clean Architecture + API Gateway)

Backend profesional de autenticación con:
- Login local (email/password)
- Login Google OAuth2/OpenID Connect
- Registro automático con Google
- JWT propio
- Refresh tokens en Redis
- Logout
- API Gateway para consumo externo
- Front básico HTML para login

## Arquitectura
- `src/AuthBackend.API`: Servicio de autenticación interno.
- `src/AuthBackend.Gateway`: API Gateway (entrada pública) + frontend básico (`wwwroot/index.html`).
- `src/AuthBackend.Application`: Casos de uso.
- `src/AuthBackend.Domain`: Entidades.
- `src/AuthBackend.Infrastructure`: PostgreSQL, Redis, seguridad, repositorios.

## Endpoints del Gateway
- `POST /api/auth/login`
- `POST /api/auth/register`
- `GET /api/auth/google`
- `GET /api/auth/google/callback`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`

## Endpoints internos Auth API
- `POST /auth/login`
- `POST /auth/register`
- `GET /auth/google`
- `GET /auth/google/callback`
- `POST /auth/refresh`
- `POST /auth/logout`

## Front básico
Abre `http://localhost:8081` y usa el formulario HTML para login básico.

## Ejemplos request/response

### POST /api/auth/register
Request:
```json
{ "email": "admin@acme.com", "password": "Secure123!" }
```
Response:
```json
{
  "accessToken": "eyJ...",
  "refreshToken": "fNn...",
  "expiresAt": "2026-03-23T10:20:30Z"
}
```

### POST /api/auth/login
Request:
```json
{ "email": "admin@acme.com", "password": "Secure123!" }
```
Response: mismo formato que register.

### GET /api/auth/google
Response:
```json
{
  "authorizationUrl": "https://accounts.google.com/o/oauth2/v2/auth?...&state=abc",
  "state": "abc"
}
```

### GET /api/auth/google/callback?id_token=...&state=...
Response: mismo formato de tokens.

### POST /api/auth/refresh
Request:
```json
{ "refreshToken": "fNn..." }
```
Response: nuevo access + refresh token.

### POST /api/auth/logout
Request:
```json
{ "refreshToken": "fNn..." }
```
Response: `204 No Content`

## Levantar con Docker
```bash
docker compose up --build
```

- Gateway: `http://localhost:8081`
- Auth API interna: `http://api:8080` (solo red interna de compose)
