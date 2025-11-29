using System;
using System.Linq;
using System.Threading.Tasks;
using ProviderOptimizer.Application.Contracts;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application.Services;

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

        if (!providers.Any())
            throw new Exception("No providers available");

        var scored = providers
            .Select(p =>
            {
                var distance = Haversine(request.Latitude, request.Longitude, p.Latitude, p.Longitude);
                var score = ComputeScore(distance, p.Rating);
                return new { Provider = p, Score = score };
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        return scored.First().Provider;
    }

    public async Task<Optimization> OptimizeAndSaveAsync(OptimizeRequestDto request)
    {
        var provider = await OptimizeAsync(request);

        var record = new Optimization
        {
            RequestName = request.AssistanceType,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Rating = request.Rating,  // ahora es double
            ProviderId = provider?.Id ?? 0
        };

        await _repo.SaveOptimizationAsync(record);

        return record;
    }

    private static double ComputeScore(double distanceKm, double rating)
    {
        double nd = Math.Max(0, 1 - (distanceKm / 100.0));
        double nr = Math.Clamp(rating / 5.0, 0.0, 1.0);
        return nr * 0.4 + nd * 0.6;
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double deg) => deg * (Math.PI / 180.0);
}
