using CertiBlock.Services.Polygon.Application.RabbitMQ;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Enums;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace CertiBlock.Services.Polygon.Application.Hangfire;

public class CheckPolygonFinalizationJob(IPolygonMetricRepository polygonMetricRepository, IPolygonRepository polygonRepository,
                                         IWeb3 web3, MetricPublisher metricPublisher, ILogger<CheckPolygonFinalizationJob> logger)
{
    private const int FinalizationBlocks = 128;

    public async Task CheckFinalizationAsync()
    {
        var unfinalizedMetrics = (await polygonMetricRepository.GetUnfinalizedMetricsAsync()).ToList();

        if (!unfinalizedMetrics.Any())
        {
            logger.LogInformation("No unfinalized Polygon metrics found.");
            return;
        }

        logger.LogInformation("Found {Count} unfinalized Polygon metrics. Checking finalization...", unfinalizedMetrics.Count);

        try
        {
            var latestBlockNumber = (long)(await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync()).Value;
            var finalizedBlockNumber = latestBlockNumber - FinalizationBlocks;

            logger.LogInformation("Latest Polygon block: {LatestBlock}, Finalized threshold: {FinalizedBlock}",
                latestBlockNumber, finalizedBlockNumber);

            int finalizedCount = 0;
            int failedCount = 0;

            foreach (var metrics in unfinalizedMetrics)
            {
                try
                {
                    if (metrics.BlockNumber <= finalizedBlockNumber)
                    {
                        var transaction = await polygonRepository
                            .GetBlockchainTransactionByCertificateIdAsync(metrics.CertificateId);

                        var submittedAt = transaction?.CreatedAt ?? DateTime.UtcNow;

                        metrics.IsFinalized = true;
                        metrics.FinalizationTimeSeconds = (DateTime.UtcNow - submittedAt).TotalSeconds;

                        await polygonMetricRepository.UpdateMetricsAsync(metrics);

                        var metricEvent = new MetricCollectedEvent(
                            metrics.CertificateId,
                            Blockchain.Polygon,
                            metrics.Operation,
                            metrics.GasUsed,
                            metrics.InclusionTimeSeconds,
                            (double)metrics.TransactionCostNative,
                            metrics.FinalizationTimeSeconds,
                            metrics.CollectedAt);

                        metricPublisher.Publish(metricEvent);
                        finalizedCount++;

                        logger.LogInformation(
                            "Marked Polygon metrics as finalized for certificate {CertificateId}. Finalization time: {FinalizationTime}s",
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
                "Polygon finalization check completed. Finalized: {FinalizedCount}, Failed: {FailedCount}, Pending: {PendingCount}",
                finalizedCount, failedCount, unfinalizedMetrics.Count - finalizedCount - failedCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching latest Polygon block");
            throw;
        }
    }
}
