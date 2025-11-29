using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application.Contracts;

public class OptimizeRequestDto
{
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string AssistanceType { get; set; } = string.Empty;
    public double Rating { get; set; }
}