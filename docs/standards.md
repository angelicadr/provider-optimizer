# Estándares técnicos del squad (resumen)

## Convenciones de código (.NET)
- Usar referencias nulas en C# 10+.
- Arquitectura limpia: Proyectos por capa (Dominio/Aplicación/Infraestructura/API).
- Usar DTO, no entidades en la capa pública.
- Preferir operaciones asíncronas/en espera (async/await) en operaciones vinculadas a E/S.
- Inserción de dependencias (DI) mediante constructor, interfaces para infraestructura.
- Escribir pruebas unitarias para servicios y pruebas de integración para interacciones con la base de datos.
- Aplicar reglas de estilo de código (formato .NET).

## React (si se aplica)
- Componentes funcionales + ganchos.
- Se recomienda Typescript.
- Carpeta por característica.

## Docker
- Compilaciones multietapa.
- Sin secretos en las imágenes.
- Usar imágenes base pequeñas (mcr.microsoft.com/dotnet/aspnet).

## Ramificación/CI
- Ramas de características (feature/*), PR a la rama principal.
- CI: ejecutar compilación, pruebas, lint, compilación de Docker.
- Proteger la rama principal con las comprobaciones necesarias.

## Secretos
- Use AWS Secrets Manager o GitHub Secrets para las canalizaciones.
- Nunca almacene credenciales en el código ni en el repositorio.

## Definición de "Listo"
- Código formateado + pruebas unitarias + pruebas de extremo a extremo/integración (si es necesario) + documentación actualizada.