using CertiBlock.Services.Ethereum.Core.Entities;

namespace CertiBlock.Services.Ethereum.Core.Repositories;

public interface IEthereumMetricRepository
{
    Task SaveEthereumMetricsAsync(EthereumMetrics ethereumMetrics);
}