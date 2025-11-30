# Arquitectura propuesta (C4 level 1-3) - Resumen

## C4 - Level 1 (System Context)

- Aplicación móvil (cliente)

- API Gateway (Autenticación, limitación de velocidad)

- Microservicios: Servicio de solicitud de asistencia, Servicio de optimización de proveedores, Servicio de notificaciones, Servicio de ubicación

- Almacenes de datos: RDS (Postgres), Redis (caché), S3 (almacenamiento)

- Agente de mensajes: SQS/SNS para eventos entre servicios

- Observabilidad: CloudWatch/Prometheus + Grafana/ELK
  
## C4 - Level 2 (Container)

- API Gateway -> rutas a microservicios (REST + gRPC)

- ProviderOptimizerService: microservicio .NET 8 (lógica de dominio) - expone/optimiza

- AssistanceRequestService: ingresa solicitudes

- NotificationsService: envía notificaciones push/sms

- LocationService: geocodificación inversa, geocercado

## C4 - Level 3 (Components) - ProviderOptimizerService

- Capa API (Controladores/API mínima)

- Aplicación (Casos de uso, DTO, Interfaces)

- Dominio (Entidades, Objetos de valor, Agregados)

- Infraestructura (Núcleo de EF, Repositorios, Clientes externos)  

## Eventos y contratos

- AssistanceRequested (generado por AssistanceRequestService)

- ProviderAssigned (generado por ProviderOptimizerService)

- ProviderNotified (generado por NotificationsService)

- Usar contratos JSON en SNS/SQS. Aplicar control de versiones del esquema.