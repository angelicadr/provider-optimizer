# create_project.ps1
# Ejecutar desde: C:\Users\linaduarte\Documents\provider-optimizer
# Uso: PowerShell: .\create_project.ps1

$base = Get-Location

Function Write-File($rel, $content) {
    $full = Join-Path $base $rel
    $dir = Split-Path $full -Parent
    if (!(Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $content | Out-File -FilePath $full -Encoding utf8 -Force
    Write-Host "WROTE: $rel"
}

# 1) ProviderOptimizer.API
Write-File "src\ProviderOptimizer.API\ProviderOptimizer.API.csproj" @"
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\\ProviderOptimizer.Application\\ProviderOptimizer.Application.csproj" />
    <ProjectReference Include="..\\ProviderOptimizer.Infrastructure\\ProviderOptimizer.Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

</Project>
"@

Write-File "src\ProviderOptimizer.API\Program.cs" @"
using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Application.Contracts;
using ProviderOptimizer.Application.Services;
using ProviderOptimizer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile(""appsettings.json"", optional: true, reloadOnChange: true);

// Connection (Docker compose uses Host=db)
var connection = builder.Configuration.GetConnectionString(""DefaultConnection"") 
                 ?? ""Host=db;Database=provider_optimizer;Username=postgres;Password=postgres"";

builder.Services.AddDbContext<OptimizerDbContext>(opts => opts.UseNpgsql(connection));
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IProviderOptimizerService, ProviderOptimizerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet(""/providers/available"", async (IProviderRepository repo) =>
{
    var providers = await repo.GetAvailableProvidersAsync();
    return Results.Ok(providers);
});

app.MapPost(""/optimize"", async (ProviderOptimizer.Application.Contracts.OptimizeRequestDto request, IProviderOptimizerService optimizer) =>
{
    var result = await optimizer.OptimizeAsync(request);
    return Results.Ok(result);
});

app.Run();
"@

Write-File "src\ProviderOptimizer.API\appsettings.json" @"
{
  ""ConnectionStrings"": {
    ""DefaultConnection"": ""Host=db;Database=provider_optimizer;Username=postgres;Password=postgres""
  }
}
"@

# 2) ProviderOptimizer.Application
Write-File "src\ProviderOptimizer.Application\ProviderOptimizer.Application.csproj" @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include=""..\\ProviderOptimizer.Domain\\ProviderOptimizer.Domain.csproj"" />
  </ItemGroup>

</Project>
"@

Write-File "src\ProviderOptimizer.Application\Contracts\IProviderRepository.cs" @"
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application.Contracts;

public interface IProviderRepository
{
    Task<IEnumerable<Provider>> GetAvailableProvidersAsync(CancellationToken ct = default);
    Task AddAsync(Provider provider, CancellationToken ct = default);
}
"@

Write-File "src\ProviderOptimizer.Application\Contracts\OptimizeRequestDto.cs" @"
namespace ProviderOptimizer.Application.Contracts;

public class OptimizeRequestDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string AssistanceType { get; set; } = string.Empty;
}
"@

Write-File "src\ProviderOptimizer.Application\Services\ProviderOptimizerService.cs" @"
using ProviderOptimizer.Application.Contracts;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application.Services;

public interface IProviderOptimizerService
{
    Task<Provider?> OptimizeAsync(OptimizeRequestDto request);
}

public class ProviderOptimizerService : IProviderOptimizerService
{
    private readonly IProviderRepository _repo;

    public ProviderOptimizerService(IProviderRepository repo)
    {
        _repo = repo;
    }

    public async Task<Provider?> OptimizeAsync(OptimizeRequestDto request)
    {
        var providers = (await _repo.GetAvailableProvidersAsync()).ToList();
        if (!providers.Any()) return null;

        var scored = providers.Select(p =>
        {
            var dist = Haversine(request.Latitude, request.Longitude, p.Latitude, p.Longitude);
            return new { Provider = p, Distance = dist, Score = ComputeScore(dist, p.Rating) };
        }).OrderByDescending(x => x.Score).ToList();

        return scored.First().Provider;
    }

    private static double ComputeScore(double distanceKm, double rating)
    {
        var nd = Math.Max(0, 1 - (distanceKm / 100.0));
        var nr = Math.Clamp(rating / 5.0, 0.0, 1.0);
        return nr * 0.4 + nd * 0.6;
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat/2)*Math.Sin(dLat/2) + Math.Cos(ToRad(lat1))*Math.Cos(ToRad(lat2))*Math.Sin(dLon/2)*Math.Sin(dLon/2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
        return R * c;
    }

    private static double ToRad(double deg) => deg * (Math.PI/180.0);
}
"@

# 3) ProviderOptimizer.Domain
Write-File "src\ProviderOptimizer.Domain\ProviderOptimizer.Domain.csproj" @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
"@

Write-File "src\ProviderOptimizer.Domain\Entities\Provider.cs" @"
namespace ProviderOptimizer.Domain.Entities;

public class Provider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsAvailable { get; set; }
    public double Rating { get; set; }
    public string[] Services { get; set; } = Array.Empty<string>();
}
"@

# 4) Update solution file (add projects if missing)
$sln = Join-Path $base 'provider-optimizer.sln'
if (Test-Path $sln) {
    Write-Host "Updating existing solution file..."
    $slnContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.7.33512.162
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ProviderOptimizer.API", "src/ProviderOptimizer.API/ProviderOptimizer.API.csproj", "{11111111-1111-1111-1111-111111111111}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ProviderOptimizer.Application", "src/ProviderOptimizer.Application/ProviderOptimizer.Application.csproj", "{22222222-2222-2222-2222-222222222222}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ProviderOptimizer.Domain", "src/ProviderOptimizer.Domain/ProviderOptimizer.Domain.csproj", "{33333333-3333-3333-3333-333333333333}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ProviderOptimizer.Infrastructure", "src/ProviderOptimizer.Infrastructure/ProviderOptimizer.Infrastructure.csproj", "{44444444-4444-4444-4444-444444444444}"
EndProject
Global
EndGlobal
"@
    $slnContent | Out-File -FilePath $sln -Encoding utf8 -Force
    Write-Host "Solution updated: provider-optimizer.sln"
} else {
    Write-Host "Solution file not found at $sln. Please ensure you're in the correct folder."
}

Write-Host "`nDONE. Los proyectos API, Application y Domain han sido creados."
Write-Host "Ahora ejecuta: dotnet restore  y luego dotnet build"
