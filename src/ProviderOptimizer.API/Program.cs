using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Application.Contracts;
using ProviderOptimizer.Application.Services;
using ProviderOptimizer.Infrastructure;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);

        var connection = builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? "Host=db;Database=provider_optimizer;Username=postgres;Password=postgres";

        builder.Services.AddDbContext<OptimizerDbContext>(opts => opts.UseNpgsql(connection));
        builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
        builder.Services.AddScoped<IProviderOptimizerService, ProviderOptimizerService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // CORS CORRECTO PARA .NET 8
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        // Habilitar CORS para TODAS las rutas
        app.UseCors("AllowAll");

        app.MapGet("/providers/available", async (IProviderRepository repo) =>
        {
            var providers = await repo.GetAvailableProvidersAsync();
            return Results.Ok(providers);
        });

        app.MapPost("/optimize", async (
            OptimizeRequestDto request,
            IProviderOptimizerService optimizer) =>
        {
            var result = await optimizer.OptimizeAndSaveAsync(request);
            return Results.Created($"/optimizations/{result.Id}", result);
        });
        
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<OptimizerDbContext>();
            db.Database.Migrate();   // Ejecuta todas las migraciones pendientes
        }

        app.Run();
    }
}