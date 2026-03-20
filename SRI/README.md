# SriFacturacion

Sistema monorepo en .NET 8 para facturación electrónica con el SRI de Ecuador, orientado a respuesta inmediata (`HTTP 202`), procesamiento asíncrono, despliegue simple en VPS Ubuntu con Docker Compose y observabilidad integrada.

## Componentes

### Servicios
- `SriFacturacion.Api`: recepción, validación, persistencia y publicación de eventos.
- `SriFacturacion.Worker`: firma, envío SOAP al SRI, consulta de autorización y publicación de factura autorizada.
- `SriEmail.Worker`: generación de PDF con QuestPDF y envío de correo con MailKit/Zoho.

### Infraestructura
- PostgreSQL
- RabbitMQ
- Redis
- Prometheus
- Loki
- Promtail
- Grafana

## Arquitectura

```text
SRI/
├── SriFacturacion.sln
├── src/
│   ├── SriFacturacion.Api
│   ├── SriFacturacion.Application
│   ├── SriFacturacion.Domain
│   ├── SriFacturacion.Infrastructure
│   ├── SriFacturacion.Worker
│   └── SriEmail.Worker
└── deploy/
    ├── grafana/
    ├── loki/
    ├── promtail/
    └── prometheus/
```

Capas internas:
- `Domain`: entidades, enums y eventos.
- `Application`: casos de uso, contratos y validadores.
- `Infrastructure`: persistencia, mensajería, SRI SOAP, cifrado, PDF, email y observabilidad.
- `API`: controladores, health checks, swagger y middlewares.

## Flujo de negocio
1. `POST /sri/facturas` valida DTO + reglas + certificado + XML.
2. Guarda la factura como `Pendiente`.
3. Publica evento en RabbitMQ.
4. El worker firma XML, lo transforma a Base64, envía al SRI y consulta autorización.
5. Guarda XML autorizado y publica `FacturaAutorizada`.
6. El email worker genera PDF y envía correo con PDF + XML.

## Endpoints

### Subir certificado
`POST /sri/certificados`

Formulario multipart:
- `archivo`: `.p12`
- `clave`
- `rucTitular`

### Crear factura
`POST /sri/facturas`

```json
{
  "numeroDocumento": "001-001-000000123",
  "rucEmisor": "1790012345001",
  "razonSocialEmisor": "Comercial Quantiva S.A.",
  "correoCliente": "cliente@correo.com",
  "identificacionCliente": "1712345678",
  "nombreCliente": "Cliente Demo",
  "moneda": "USD",
  "certificadoId": "00000000-0000-0000-0000-000000000000",
  "items": [
    {
      "codigoPrincipal": "SKU-001",
      "descripcion": "Servicio mensual",
      "cantidad": 1,
      "precioUnitario": 100,
      "porcentajeDescuento": 0,
      "codigoImpuesto": "2"
    }
  ]
}
```

### Consultar estado
`GET /sri/facturas/{id}`

## Seguridad
- Contraseñas de certificados cifradas con AES-256.
- Secretos por variables de entorno.
- Directorios persistentes separados para certificados y XML autorizados.
- Logs estructurados sin exponer secretos.
- Preparado para TLS desde reverse proxy o certificado montado en Kestrel.

## Observabilidad
- Logs JSON con Serilog.
- CorrelationId y RequestId en middleware.
- `/metrics` en API y workers para Prometheus.
- `/health`, `/health/live`, `/health/ready` para PostgreSQL, RabbitMQ y Redis.
- Dashboard de Grafana provisionado automáticamente.
- Centralización de logs con Promtail → Loki.

## Despliegue

1. Copiar variables:
   ```bash
   cp .env.example .env
   ```
2. Reemplazar `AES_KEY_BASE64` por una llave Base64 de 32 bytes.
3. Configurar credenciales SMTP de Zoho.
4. Levantar la plataforma:
   ```bash
   docker compose up -d --build
   ```

## URLs útiles
- API: `http://TU_HOST:8080/swagger`
- RabbitMQ: `http://TU_HOST:15672`
- Grafana: `http://TU_HOST:3000`
- Prometheus: `http://TU_HOST:9090`
- Worker health: `http://TU_HOST:8081/health`
- Email worker health: `http://TU_HOST:8082/health`

## Notas operativas
- Las migraciones SQL se ejecutan automáticamente al inicio de cada servicio.
- Los catálogos tributarios se leen desde la tabla `Catalogos`; el IVA no está hardcodeado en la lógica de negocio.
- Los reintentos con Polly están reservados para fallos técnicos de SRI y SMTP.
- Las colas usan Dead Letter Queue por cada routing key principal.

## Siguientes endurecimientos recomendados
- Montar certificado TLS real para Kestrel o colocar Nginx/Caddy delante del API.
- Sustituir el XSD embebido por los XSD oficiales versionados del SRI dentro de `Infrastructure/Recursos`.
- Agregar pruebas automáticas de integración con RabbitMQ/PostgreSQL mediante Testcontainers.
