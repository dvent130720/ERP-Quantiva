# Quantiva SRI Platform (MARZO)

Plataforma **multi-tenant** para facturación electrónica SRI lista para evolucionar a producción.

## Arquitectura aplicada

- Clean Architecture + DDD + SOLID en `Billing.Service`.
- Microservicios separados:
  - Gateway (`YARP`)
  - Identity Service (JWT + refresh token)
  - Tenant Service (multi-RUC + certificados)
  - Billing Service (core SRI)
  - Worker Service (procesamiento asíncrono)
  - Notification Service
- Event-driven con RabbitMQ (`billing.events`).
- PostgreSQL como persistencia principal de comprobantes, Redis para cache.
- Resiliencia base con retry exponencial (Polly) en worker.

## Flujo de eventos

1. `POST /facturas` en Billing Service crea comprobante y publica `FacturaCreada`.
2. Worker consume y ejecuta pipeline (XML, firma, envío SRI, autorización).
3. Se actualiza estado y se publican eventos de resultado:
   - `FacturaFirmada`
   - `FacturaEnviada`
   - `FacturaAutorizada`
   - `FacturaError`
4. Notification Service consume `FacturaAutorizada` para webhook/email.

## Endpoints clave

- `POST /facturas`
- `GET /facturas/{id}`
- `GET /facturas/{clave}/estado`
- `POST /internal/facturas/{id}/estado` (uso interno worker)
- `POST /identity/token`
- `POST /tenants`

## SRI PRUEBAS (configurable)

- Recepción: `https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl`
- Autorización: `https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl`

> No se hardcodean secrets: usar variables de entorno/secret manager (Vault o AWS Secrets Manager) para producción.

## Ejecución local

```bash
cd ERPSystem/microservices
export JWT_SECRET='change-me-please'
docker compose up --build
```

Gateway expone en `http://localhost:8080`.

## Producción

- CI/CD: `.github/workflows/microservices-ci.yml`
- Kubernetes ready: `k8s/billing-deployment.yaml`
- Añadir observabilidad:
  - Logs: Loki/ELK
  - Métricas: Prometheus + Grafana
- Recomendado:
  - Outbox pattern
  - Idempotency store distribuido
  - Firma XAdES-BES real mediante librería certificada
  - cifrado de `.p12` con KMS


## 🧪 Pruebas en Postman

- Guía paso a paso: `docs/Postman-Pruebas-Flujo-SRI.md`
- Colección importable: `docs/Postman-Collection-MARZO.json`
- Formulario web simple para alta con `.p12`: `docs/index.html`
