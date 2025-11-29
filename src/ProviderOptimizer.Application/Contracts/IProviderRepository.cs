using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProviderOptimizer.Domain.Entities;

namespace ProviderOptimizer.Application.Contracts
{
    public interface IProviderRepository
    {

        Task<IEnumerable<Provider>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Provider provider, CancellationToken cancellationToken = default);
        Task SaveOptimizationAsync(Optimization opt);

    }
}
