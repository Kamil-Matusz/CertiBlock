using CertiBlock.Services.Ethereum.Core.Repositories;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace CertiBlock.Services.Ethereum.Application.Hangfire;

public class CheckEthereumFinalizationJob(IEthereumMetricRepository ethereumMetricRepository, IEthereumRepository ethereumRepository,
                                          IWeb3 web3, ILogger<CheckEthereumFinalizationJob> logger)
{
    private const int FinalizationBlocks = 64;

    public async Task CheckFinalizationAsync()
    {
        var unfinalizedMetrics = (await ethereumMetricRepository.GetUnfinalizedMetricsAsync()).ToList();

        if (!unfinalizedMetrics.Any())
        {
            logger.LogInformation("No unfinalized Ethereum metrics found.");
            return;
        }

        logger.LogInformation("Found {Count} unfinalized Ethereum metrics. Checking finalization...", unfinalizedMetrics.Count);

        try
        {
            var latestBlockNumber = (long)(await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
            var finalizedBlockNumber = latestBlockNumber - FinalizationBlocks;

            logger.LogInformation("Latest Ethereum block: {LatestBlock}, Finalized threshold: {FinalizedBlock}",
                latestBlockNumber, finalizedBlockNumber);

            int finalizedCount = 0;
            int failedCount = 0;

            foreach (var metrics in unfinalizedMetrics)
            {
                try
                {
                    if (metrics.BlockNumber <= finalizedBlockNumber)
                    {
                        var transaction = await ethereumRepository
                            .GetBlockchainTransactionByCertificateIdAsync(metrics.CertificateId);

                        var submittedAt = transaction?.CreatedAt ?? DateTime.UtcNow;

                        metrics.IsFinalized = true;
                        metrics.FinalizationTimeSeconds = (DateTime.UtcNow - submittedAt).TotalSeconds;

                        await ethereumMetricRepository.UpdateMetricsAsync(metrics);
                        finalizedCount++;

                        logger.LogInformation(
                            "Marked Ethereum metrics as finalized for certificate {CertificateId}. Finalization time: {FinalizationTime}s",
                            metrics.CertificateId, metrics.FinalizationTimeSeconds);
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    logger.LogError(ex, "Error checking finalization for certificate {CertificateId}", metrics.CertificateId);
                }
            }

            logger.LogInformation(
                "Ethereum finalization check completed. Finalized: {FinalizedCount}, Failed: {FailedCount}, Pending: {PendingCount}",
                finalizedCount, failedCount, unfinalizedMetrics.Count - finalizedCount - failedCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching latest Ethereum block");
            throw;
        }
    }
}
