# Documento de Pruebas en Postman - Flujo Multi-tenant SRI

Este documento describe cómo probar el flujo actual de microservicios con Postman, incluyendo:

- Registro de una empresa (primer alta de tenant).
- Obtención de token de autenticación.
- Emisión y consulta de factura.
- Casos de uso sugeridos y casos de prueba.

> Alcance: guía práctica para el estado actual del código en `ERPSystem/microservices`.

---

## 1) Prerrequisitos

1. Levantar la plataforma:

```bash
cd ERPSystem/microservices
export JWT_SECRET='change-me-please'
# Clave AES-256 en Base64 (32 bytes)
export P12_MASTER_KEY='MDEyMzQ1Njc4OUFCQ0RFRjAxMjM0NTY3ODlBQkNERUY='
docker compose up --build
```

2. Tener un certificado `.p12` de pruebas y su password.
3. Convertir el archivo `.p12` a Base64 para enviarlo en JSON:

```bash
base64 -w 0 empresa-certificado.p12
```

---

## 2) Configuración recomendada de Postman

Crear un **Environment** llamado `MARZO-LOCAL` con variables:

- `gateway_url` = `http://localhost:8080`
- `identity_url` = `http://localhost:8080/identity`
- `tenant_url` = `http://localhost:8080/tenants`
- `billing_url` = `http://localhost:8080/facturas`
- `api_key` = `demo-key-tenant`
- `tenant_id` = (vacío inicialmente)
- `access_token` = (vacío inicialmente)
- `p12_base64` = (valor del certificado base64)
- `p12_password` = (password del .p12)
- `ruc` = `1790012345001`

### Headers comunes (colección)

- `X-Api-Key: {{api_key}}`
- `Content-Type: application/json`

> Si quieres pasar por Gateway para todas las rutas, usa `{{gateway_url}}` como host base.

---

## 3) Flujo principal de uso (end-to-end)

## Paso A: Registro de empresa por primera vez (tenant)

### Request

**POST** `{{tenant_url}}`

Body:

```json
{
  "tenantId": "{{$guid}}",
  "ruc": "{{ruc}}",
  "businessName": "Empresa Demo S.A.",
  "p12Base64": "{{p12_base64}}",
  "p12Password": "{{p12_password}}",
  "sriEnvironment": "PRUEBAS"
}
```

### Validaciones esperadas

- HTTP `201 Created`.
- Respuesta con `tenantId`.
- Guardar `tenantId` en variable de entorno `tenant_id`.

### Script Postman (Tests)

```javascript
pm.test("Tenant creado", function () {
  pm.response.to.have.status(201);
});

const body = pm.response.json();
if (body.tenantId) {
  pm.environment.set("tenant_id", body.tenantId);
}
```

---

## Paso B: Solicitar token JWT

### Request

**POST** `{{identity_url}}/token`

Body:

```json
{
  "userName": "admin@demo.com",
  "password": "Pass.123",
  "tenantId": "{{tenant_id}}"
}
```

### Validaciones esperadas

- HTTP `200 OK`.
- Respuesta con `access_token` y `refresh_token`.

### Script Postman (Tests)

```javascript
pm.test("Token generado", function () {
  pm.response.to.have.status(200);
});

const body = pm.response.json();
if (body.access_token) {
  pm.environment.set("access_token", body.access_token);
}
```

> En el estado actual, varias rutas usan API Key + tenant; el JWT queda listo para cuando se endurezca auth en gateway/servicios.

---

## Paso C: Emitir factura

### Request

**POST** `{{billing_url}}`

Headers extra:

- `Authorization: Bearer {{access_token}}`
- `X-Tenant-Id: {{tenant_id}}`

Body:

```json
{
  "tenantId": "{{tenant_id}}",
  "ruc": "{{ruc}}",
  "secuencial": "1",
  "total": 145.67,
  "buyerDocument": "0912345678",
  "buyerName": "Cliente Prueba"
}
```

### Validaciones esperadas

- HTTP `202 Accepted`.
- Respuesta con `id`, `claveAcceso`, `estado`.
- `estado` inicial esperado: `CREADA`.

### Script Postman (Tests)

```javascript
pm.test("Factura aceptada", function () {
  pm.response.to.have.status(202);
});

const body = pm.response.json();
if (body.id) pm.environment.set("invoice_id", body.id);
if (body.claveAcceso) pm.environment.set("clave_acceso", body.claveAcceso);
```

---

## Paso D: Consultar factura por ID

### Request

**GET** `{{billing_url}}/{{invoice_id}}`

Validar:

- HTTP `200 OK`.
- Presencia de `id`, `claveAcceso`, `tenantId`.

---

## Paso E: Consultar estado por clave de acceso

### Request

**GET** `{{billing_url}}/{{clave_acceso}}/estado`

Validar:

- HTTP `200 OK`.
- `estado` y `errores`.

---

## 4) Casos de uso recomendados para pruebas

## CU-01 Alta inicial de empresa

- Objetivo: registrar tenant con certificado cifrado.
- Endpoint: `POST /tenants`.
- Éxito: `201` + `tenantId`.

## CU-02 Emisión inicial de factura

- Objetivo: crear comprobante y publicar evento `FacturaCreada`.
- Endpoint: `POST /facturas`.
- Éxito: `202` + `claveAcceso`.

## CU-03 Consulta de trazabilidad

- Objetivo: revisar estado por ID y por clave.
- Endpoints: `GET /facturas/{id}` y `GET /facturas/{clave}/estado`.

## CU-04 Manejo de duplicados

- Ejecutar 2 veces `POST /facturas` con mismos datos (tenant + secuencial + fecha).
- Esperado: segunda llamada `409 Conflict`.

## CU-05 Validación de seguridad mínima

- Quitar `X-Api-Key` y reintentar.
- Esperado desde gateway: `401 Missing API Key`.

---

## 5) Casos negativos sugeridos

1. **Tenant inexistente en token**
   - `POST /identity/token` con `tenantId` no creado.
2. **Factura sin campos obligatorios**
   - omitir `ruc` o `secuencial`.
3. **API key ausente o vacía**
   - debe bloquear gateway.
4. **Clave de acceso inexistente**
   - `GET /facturas/{clave}/estado` retorna `404`.

---

## 6) Recomendación de organización de colección Postman

Carpetas sugeridas:

1. `01-Tenants`
   - `POST Crear tenant`
   - `GET Obtener tenant`
2. `02-Identity`
   - `POST Token`
3. `03-Billing`
   - `POST Crear factura`
   - `GET Factura por ID`
   - `GET Estado por clave`
4. `99-Negativos`

---

## 7) Tips de operación

- Reutiliza `tenant_id` como string en todo el flujo.
- Si pruebas certificados reales, evita subir `p12_base64` a workspaces compartidos.
- Para automatización, agrega esta colección en Newman dentro de CI cuando el ambiente tenga contenedores activos.
