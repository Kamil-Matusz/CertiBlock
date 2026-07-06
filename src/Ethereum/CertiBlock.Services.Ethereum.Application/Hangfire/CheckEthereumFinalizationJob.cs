using CertiBlock.Services.Ethereum.Application.RabbitMQ;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace CertiBlock.Services.Ethereum.Application.Hangfire;

public class CheckEthereumFinalizationJob(IEthereumMetricRepository metricsRepository, IWeb3 web3,
    ILogger<CheckEthereumFinalizationJob> logger, MetricPublisher metricPublisher)
{
    private const int RequiredConfirmations = 64;

    public async Task CheckFinalizationAsync()
    {
        var unfinalizedMetrics = (await metricsRepository.GetUnfinalizedMetricsAsync()).ToList();

        if (!unfinalizedMetrics.Any())
        {
            logger.LogInformation("No unfinalized Ethereum metrics found.");
            return;
        }

        var currentBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
        var currentBlockNumber = (long)currentBlock.Value;

        logger.LogInformation("Checking finalization for {Count} Ethereum metrics. Current block: {Block}",
                               unfinalizedMetrics.Count, currentBlockNumber);

        int finalizedCount = 0;
        int failedCount = 0;

        foreach (var metric in unfinalizedMetrics)
        {
            try
            {
                if (metric.BlockNumber <= 0)
                {
                    logger.LogWarning("Skipping metric {MetricId} - missing BlockNumber.", metric.Id);
                    continue;
                }

                var confirmations = currentBlockNumber - metric.BlockNumber;

                if (confirmations < RequiredConfirmations)
                    continue;

                var txBlock = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber
                    .SendRequestAsync(new Nethereum.Hex.HexTypes.HexBigInteger(metric.BlockNumber));
                var txBlockTimestamp = DateTimeOffset.FromUnixTimeSeconds((long)txBlock.Timestamp.Value).UtcDateTime;

                var finalityBlockNumber = metric.BlockNumber + RequiredConfirmations;
                var finalityBlock = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber
                    .SendRequestAsync(new Nethereum.Hex.HexTypes.HexBigInteger(finalityBlockNumber));
                var finalityBlockTimestamp = DateTimeOffset.FromUnixTimeSeconds((long)finalityBlock.Timestamp.Value).UtcDateTime;

                var confirmationWindowSeconds = (finalityBlockTimestamp - txBlockTimestamp).TotalSeconds;
                metric.FinalizationTimeSeconds = metric.InclusionTimeSeconds + confirmationWindowSeconds;
                metric.IsFinalized = true;
                metric.Confirmations = RequiredConfirmations;

                var finalizedEvent = new MetricFinalizedEvent(
                    metric.CertificateId,
                    Shared.Enums.Blockchain.Ethereum,
                    metric.FinalizationTimeSeconds!.Value,
                    metric.Confirmations,
                    finalityBlockTimestamp);
                metricPublisher.Publish(finalizedEvent);

                await metricsRepository.UpdateEthereumMetricsAsync(metric);

                finalizedCount++;

                logger.LogInformation("Ethereum metric {MetricId} finalized. Confirmations: {Confirmations}, " +
                                      "FinalizationTime: {Time}s", metric.Id, confirmations, metric.FinalizationTimeSeconds);

                await Task.Delay(300);
            }
            catch (Exception ex)
            {
                failedCount++;
                logger.LogError(ex, "Error checking finalization for Ethereum metric {MetricId}", metric.Id);
            }
        }

        logger.LogInformation("Ethereum finalization check completed. Finalized: {Finalized}, Failed: {Failed}, " +
                              "Total checked: {Total}", finalizedCount, failedCount, unfinalizedMetrics.Count);
    }
}
