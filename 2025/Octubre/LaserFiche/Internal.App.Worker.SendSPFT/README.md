# Nombre de la aplicación o el servicio

|||
| :-------------------- | :------ |
| Dominio (Namespace):  | [Domain Name]    |
| Sub Dominio:          | [Sub Domain Name]|
| Módulo (optional):    | {Module Name}    |
| Depende de:           | [List the services you depend on]  |
| Protocolo:            | [HTTP/AMQP/TCP]   |
| URL:                  | [Service or Application URL]|
|||

## Propósito

_Explique clara mente el propósito de su aplicación o servicio.  Sea claro, es importante que aporte los links a los documentos de la metodología Davivienda, que permitan aumentar la claridad de su desarrollo._

## Flujo de ejecución

_En esta sección agregue los diagramas de secuencia que permitan ejemplificar la secuencia de ejecución de las transacciones de su aplicación o servicio.  Utilice notación **UML estándar**._

_**Importante**:_

_1. Como estándar en Davivienda se utiliza la herramienta ==Draw.io== para la construcción de diagramas, no esta permitido el uso de ninguna otra._
_2. Los diagramas de arquitectura, se deben dibujar con la nomenclatura estándar promocionada por Davivienda Colombia._

## Configuración y Despliegue

_En esta sección incluya todas las instrucciones y consideraciones que crea necesarias recordar para que el proceso de configuración y despliegue de su aplicación o servicio sea un éxito_

## Información de soporte

_En esta sección incluya todo lo requerido para poder diagnosticar y atender cualquier incidentes sobre la aplicación o servicio.
Sea claro y conciso, esta documentación le puede ser util a usted mismo en el futuro._

## Healthcheck (Requerido para servicios)

|||
| :-------------------- | :------ |
| EndPoint:             | [Service or Application URL]    |
| Reglas:               | Explique claramente como se debe interpretar la respuesta del servicio |
|||

Es requerido que documente la respuesta de su chequeo de salud de esta forma:

```json
{
    "Status": "Healthy",
    "HealthChecks": [
        {
            "Status": "Healthy",
            "Component": "Redis",
            "Description": null
        }
    ],
    "HealthCheckDuration": "00:00:01.0018353"
}
```

>Use el fragmento JSON como guía, reemplace por el resultado de su servicio.  ELIMINE ESTE TEXTO.
>
>Recuerde siempre utilizar el identificador de código

## Request / Response

_Si esta documentando un servicio, Stored Procedure, Archivo, Mensaje, o cualquier otro tipo de interfaz, es indispensable que porte el link al documento "FOR-TI-14-002 Documentación de Interfaz"._
