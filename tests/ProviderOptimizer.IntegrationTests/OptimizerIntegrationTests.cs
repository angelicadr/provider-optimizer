using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Infrastructure;
using ProviderOptimizer.Domain.Entities;
using ProviderOptimizer.Application.Services;
using ProviderOptimizer.Application.Contracts;
using System.Collections.Generic;

public class OptimizerIntegrationTests
{
    [Fact]
    public async Task Optimize_Integration_Works_With_InMemoryDb()
    {
        var options = new DbContextOptionsBuilder<OptimizerDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var db = new OptimizerDbContext(options);

        db.Providers.Add(new Provider
        {
            Name = "P1",
            Latitude = 1,
            Longitude = 1,
            IsAvailable = true,
            Rating = 4.5,
            Services = new List<string> { "grua" }
        });

        db.Providers.Add(new Provider
        {
            Name = "P2",
            Latitude = 10,
            Longitude = 10,
            IsAvailable = true,
            Rating = 5.0,
            Services = new List<string> { "grua" }
        });

        await db.SaveChangesAsync();

        IProviderRepository repo = new ProviderRepository(db);
        var svc = new ProviderOptimizerService(repo);

        var request = new OptimizeRequestDto
        {
            Latitude = 1.1,
            Longitude = 1.1,
            AssistanceType = "grua"
        };

        var chosen = await svc.OptimizeAsync(request);

        Assert.NotNull(chosen);
    }
}