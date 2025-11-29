# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto primero para aprovechar caching
COPY src/ProviderOptimizer.Domain/*.csproj src/ProviderOptimizer.Domain/
COPY src/ProviderOptimizer.Application/*.csproj src/ProviderOptimizer.Application/
COPY src/ProviderOptimizer.Infrastructure/*.csproj src/ProviderOptimizer.Infrastructure/
COPY src/ProviderOptimizer.API/*.csproj src/ProviderOptimizer.API/
COPY src/ProviderOptimizer.API/ ./src/ProviderOptimizer.API/
COPY tests/ProviderOptimizer.UnitTests/*.csproj tests/ProviderOptimizer.UnitTests/
COPY tests/ProviderOptimizer.IntegrationTests/*.csproj tests/ProviderOptimizer.IntegrationTests/
COPY provider-optimizer.sln .

# RESTORE dentro del contenedor 🚀
RUN dotnet restore provider-optimizer.sln

# Copiar el resto del código
COPY . .

# Ahora sí, PUBLICAR correctamente
RUN dotnet publish src/ProviderOptimizer.API/ProviderOptimizer.API.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProviderOptimizer.API.dll"]
