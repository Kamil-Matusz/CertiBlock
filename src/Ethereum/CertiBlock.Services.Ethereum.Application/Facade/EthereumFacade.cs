using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;

namespace CertiBlock.Services.Ethereum.Application.Facade;

public class EthereumFacade(IEthereumService ethereumService, IEthereumMetricService ethereumMetricService) : IEthereumFacade
{
    public async Task DeleteEthereumTransactionWithMetricsAsync(Guid certificateId)
    {
        await ethereumService.DeleteEthereumTransactionByCertificateIdAsync(certificateId);
        await ethereumMetricService.DeleteTransactionMetricsByCertificateIdAsync(certificateId);
    }
}