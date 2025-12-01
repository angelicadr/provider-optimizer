# ProviderOptimizerService - Solution (Sample for Technical Test)

Esta carpeta contiene la solución mínima requerida por la prueba técnica para el módulo
**Asignación Inteligente de Proveedores** (ProviderOptimizerService). Incluye:
- Microservicio .NET 8 (API minimal)
- Clean Architecture (Domain / Application / Infrastructure / API)
- Endpoints: POST /optimize, GET /providers/available, GET /tracking/{requestId}
- Persistence: EF Core (Postgres suggested). Migration SQL included.
- Frontend React (provider-optimizer-ui)
- Dockerfile + docker-compose.yml
- GitHub Actions workflow (ci.yml)
- Unit tests (xUnit) and an integration test (InMemory provider)
- Documentación: arquitectura, estándares, code review
- Snippet defectuoso y revisión

## Cómo ejecutar localmente (sugerido)
1. Tener .NET 8 SDK instalado.
2. Ajustar `appsettings.Development.json` con la cadena de conexión a Postgres si desea usar DB real.
3. Con .NET (sin Docker) Ejecutar:
   - `dotnet restore`
   - `dotnet build src/ProviderOptimizer.API/ProviderOptimizer.API.csproj`
   - `dotnet run --project src/ProviderOptimizer.API/ProviderOptimizer.API.csproj`
4. Alternativamente, Con Docker (recomendado para reproducibilidad):
   - `docker compose up --build`
   - `docker compose down`  
5. Servicios:
   - Postgres expuesto en 5432 (interna al compose está en db).
   - API en http://localhost:5000 (mapeado al puerto 80 del contenedor).

6. El script migrations/init.sql se monta en el contenedor Postgres y crean las tablas providers y optimizations al iniciar.

Ejecutar tests:

dotnet test desde la raíz (ejecuta proyectos de tests/).

## Contenido
Revisa `docs/` para la arquitectura, evidencia de pruebas y `standards/` para políticas de equipo.

## URL de la aplicación (Frontend + Backend)
Aplicación Web (Frontend – React + Nginx)
http://localhost:3000

API Backend (.NET 8 – ProviderOptimizer.API)
http://localhost:5000

Swagger UI:
http://localhost:5000/swagger

Base de datos PostgreSQL
Host: localhost
Puerto: 5432
DB: provider_optimizer