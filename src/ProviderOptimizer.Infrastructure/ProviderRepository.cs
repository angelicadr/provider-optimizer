using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizer.Application.Contracts;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Infrastructure
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly OptimizerDbContext _context;

        public ProviderRepository(OptimizerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Provider>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Providers
                .Where(p => p.IsAvailable)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Provider provider, CancellationToken cancellationToken = default)
        {
            await _context.Providers.AddAsync(provider, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveOptimizationAsync(Optimization opt)
        {
            _context.Optimizations.Add(opt);
            await _context.SaveChangesAsync();
        }
    }
}
