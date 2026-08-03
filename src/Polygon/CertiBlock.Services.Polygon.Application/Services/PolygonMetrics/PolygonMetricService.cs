using CertiBlock.Services.Polygon.Application.Mappers;
using CertiBlock.Services.Polygon.Application.RabbitMQ;
using CertiBlock.Services.Polygon.Application.Services.CoinGecko;
using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Services.Polygon.Core.Exceptions;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Enums;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;

public class PolygonMetricService(IPolygonMetricRepository polygonMetricRepository, ILogger<PolygonMetricService> logger,
    IWeb3 web3, ICoinGeckoService coinGeckoService, MetricPublisher metricPublisher,
    IPolygonRepository polygonRepository) : IPolygonMetricService
{
    public async Task<Core.Entities.PolygonMetrics> CollectMetricsAsync(Guid certificateId, string transactionHash)
    {
        try
        {
            var existingMetrics = await polygonMetricRepository.GetTransactionMetricsByCertificateAsync(certificateId);
            if (existingMetrics is not null)
            {
                logger.LogInformation("Metrics for certificate {CertificateId} already collected - republishing event", certificateId);
                metricPublisher.Publish(CreateMetricEvent(existingMetrics));
                return existingMetrics;
            }

            var txn = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            var latestBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            if (txn == null || receipt == null)
                throw new PolygonTransactionsByHashNotFoundException($"Transaction {transactionHash} not found.");

            var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(receipt.BlockNumber);
            var blockTimestamp = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).UtcDateTime;

            var transaction = await polygonRepository.GetBlockchainTransactionByCertificateIdAsync(certificateId);
            var submittedAt = transaction?.CreatedAt ?? blockTimestamp;
            var inclusionTimeSeconds = (blockTimestamp - submittedAt).TotalSeconds;

            var dataSizeBytes = string.IsNullOrEmpty(txn.Input) ? 0 : (txn.Input.Length - 2) / 2;
            var confirmations = (int)(latestBlock.Value - receipt.BlockNumber.Value);
            var gasUsed = (long)receipt.GasUsed.Value;
            var effectiveGasPrice = (decimal)receipt.EffectiveGasPrice.Value;
            var transactionCostNative = (gasUsed * effectiveGasPrice) / 1_000_000_000_000_000_000m;
            
            var assetId = "polygon-ecosystem-token";
            var maticUsdPrice = await coinGeckoService.GetPriceUsdAsync(assetId);
            var transactionCostUsd = transactionCostNative * maticUsdPrice;
            
            var gasUtilizationRatio = txn.Gas.Value > 0
                ? (double)gasUsed / (double)txn.Gas.Value * 100.0
                : 0;

            var collectedAt = DateTime.UtcNow;

            var metrics = new Core.Entities.PolygonMetrics
            {
                CertificateId = certificateId,
                Blockchain = Blockchain.Polygon,
                Operation = Operation.Register,
                TransactionHash = transactionHash,
                DataSizeBytes = dataSizeBytes,
                Confirmations = confirmations,
                TransactionCostNative = transactionCostNative,
                TransactionCostUsd = transactionCostUsd,
                GasUsed = gasUsed,
                GasUtilizationRatio = gasUtilizationRatio,
                InclusionTimeSeconds = inclusionTimeSeconds,
                BlockNumber = (long)receipt.BlockNumber.Value,
                IsFinalized = false,
                FinalizationTimeSeconds = null,
                CollectedAt = collectedAt
            };

            await polygonMetricRepository.SavePolygonMetricsAsync(metrics);

            metricPublisher.Publish(CreateMetricEvent(metrics));

            return metrics;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error collecting Polygon metrics for transaction {TransactionHash}", transactionHash);
            throw;
        }
    }

    private static MetricCollectedEvent CreateMetricEvent(Core.Entities.PolygonMetrics metrics)
        => new(
            metrics.CertificateId,
            Blockchain.Polygon,
            Operation.Register,
            metrics.GasUsed,
            metrics.InclusionTimeSeconds,
            (double)metrics.TransactionCostUsd,
            metrics.CollectedAt);

    public async Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId)
    {
        var polygonTransaction = await polygonMetricRepository.GetTransactionMetricsByCertificateAsync(certificateId);
        if (polygonTransaction is null)
        {
            throw new PolygonTransactionsNotFoundException(certificateId);
        }

        await polygonMetricRepository.DeleteTransactionMetricsByCertificateIdAsync(certificateId);
    }

    public async Task<PolygonMetricDetailsDto> GetTransactionMetricsByCertificateIdAsync(Guid certificateId)
    {
        var polygonTransaction = await polygonMetricRepository.GetTransactionMetricsByCertificateAsync(certificateId);
        if (polygonTransaction is null)
        {
            throw new PolygonTransactionsByCertificateIdNotFoundException(certificateId);
        }

        return PolygonMetricsMapper.Map<PolygonMetricDetailsDto>(polygonTransaction);
    }

    public async Task<IEnumerable<PolygonMetricResearchDto>> GetAllResearchMetricsAsync()
    {
        return await polygonMetricRepository.GetAllResearchMetricsAsync();
    }
}