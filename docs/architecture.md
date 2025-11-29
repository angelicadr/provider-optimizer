# Arquitectura propuesta (C4 level 1-3) - Resumen

## C4 - Level 1 (System Context)
- Mobile App (cliente)
- API Gateway (Auth, rate limiting)
- Microservices: AssistanceRequestService, ProviderOptimizerService, NotificationsService, LocationService
- Datastores: RDS (Postgres), Redis (cache), S3 (storage)
- Message broker: SQS / SNS for events between services
- Observability: CloudWatch / Prometheus + Grafana / ELK

## C4 - Level 2 (Container)
- API Gateway -> routes to microservices (REST + gRPC)
- ProviderOptimizerService: .NET 7 microservice (domain logic) - exposes /optimize
- AssistanceRequestService: ingresa solicitudes
- NotificationsService: envía push / sms
- LocationService: reverse geocoding, geo-fencing

## C4 - Level 3 (Components) - ProviderOptimizerService
- API Layer (Controllers / Minimal API)
- Application (UseCases, DTOs, Interfaces)
- Domain (Entities, ValueObjects, Aggregates)
- Infrastructure (EF Core, Repositories, External clients)

## Eventos y contratos
- Events:
  - AssistanceRequested (produced by AssistanceRequestService)
  - ProviderAssigned (produced by ProviderOptimizerService)
  - ProviderNotified (produced by NotificationsService)
- Use JSON contracts on SNS/SQS. Apply schema versioning.

