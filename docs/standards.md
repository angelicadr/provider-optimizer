# Estándares técnicos del squad (resumen)

## Convenciones de código (.NET)
- Usar C# 10+ nullable references enabled.
- Clean Architecture: Projects by layer (Domain / Application / Infrastructure / API).
- Use DTOs, no entidades en la capa pública.
- Prefer async/await en IO-bound operations.
- DI via constructor, interfaces for infra.
- Write unit tests for services and integration tests for DB interactions.
- Apply code style rules (dotnet format).

## React (si aplica)
- Functional components + hooks.
- Typescript recomendado.
- Folder by feature.

## Docker
- Multi-stage builds.
- No secrets in images.
- Use small base images (mcr.microsoft.com/dotnet/aspnet).

## Branching / CI
- Feature branches (feature/*), PR to main.
- CI: run build, tests, lint, docker build.
- Protect main branch with required checks.

## Secrets
- Use AWS Secrets Manager or GitHub Secrets for pipelines.
- Never store credentials in code o repo.

## Definition of Done
- Code formatted + unit tests + e2e/integration tests (if needed) + docs updated.
