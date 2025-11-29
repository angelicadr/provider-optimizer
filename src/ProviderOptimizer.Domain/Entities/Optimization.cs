using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProviderOptimizer.Domain.Entities;

public class Optimization
{
    public int Id { get; set; }

    public string RequestName { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double Rating { get; set; }

    public int ProviderId { get; set; }

    public Provider Provider { get; set; } = null!;
}
