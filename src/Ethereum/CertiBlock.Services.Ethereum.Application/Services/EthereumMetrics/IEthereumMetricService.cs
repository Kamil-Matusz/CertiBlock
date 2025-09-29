using CertiBlock.Services.Ethereum.Core.DTO;

namespace CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;

public interface IEthereumMetricService
{
    Task<Core.Entities.EthereumMetrics> CollectMetricsAsync(Guid certificateId, string transactionHash);
    Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId);
    Task<EthereumMetricDetailsDto> GetTransactionMetricsByCertificateIdAsync(Guid certificateId);
}