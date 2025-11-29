using System.Threading.Tasks;
using System;
using ProviderOptimizer.Application.Contracts;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application.Contracts;

public interface IProviderOptimizerService
{
    Task<Provider?> OptimizeAsync(OptimizeRequestDto request);
    Task<Optimization> OptimizeAndSaveAsync(OptimizeRequestDto request);
}
