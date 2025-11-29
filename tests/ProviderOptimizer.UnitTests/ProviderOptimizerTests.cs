using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ProviderOptimizer.Application.Contracts;
using ProviderOptimizer.Application.Services;
using ProviderOptimizer.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class ProviderOptimizerTests
{
    [Fact]
    public async Task OptimizeAsync_Chooses_Closest_Highrating()
    {
        var repoMock = new Mock<IProviderRepository>();
        repoMock.Setup(r => r.GetAvailableProvidersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Provider> {
                new Provider { Name = "NearLow", Latitude = 10, Longitude = 10, IsAvailable = true, Rating = 3.0 },
                new Provider { Name = "FarHigh", Latitude = 50, Longitude = 50, IsAvailable = true, Rating = 5.0 },
                new Provider { Name = "VeryNear", Latitude = 10.001, Longitude = 10.001, IsAvailable = true, Rating = 4.0 }
            });

        var service = new ProviderOptimizerService(repoMock.Object);

        var request = new OptimizeRequestDto
        {
            Latitude = 10,
            Longitude = 10,
            AssistanceType = "grua"
        };

        var result = await service.OptimizeAsync(request);

        Assert.NotNull(result);
        Assert.Equal("VeryNear", result!.Name);
    }
}