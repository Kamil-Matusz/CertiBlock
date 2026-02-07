using CertiBlock.Services.Ethereum.Core.Entities;

namespace CertiBlock.Services.Ethereum.Core.Repositories;

public interface IEthereumMetricRepository
{
    Task SaveEthereumMetricsAsync(EthereumMetrics ethereumMetrics);
    Task<EthereumMetrics> GetTransactionMetricsByCertificateAsync(Guid certificateId);
    Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId);
    Task<IEnumerable<Guid>> GetAllCertificateIdsWithMetricsAsync();
    Task<IEnumerable<EthereumMetrics>> GetUnfinalizedMetricsAsync();
    Task UpdateEthereumMetricsAsync(EthereumMetrics metrics);
}