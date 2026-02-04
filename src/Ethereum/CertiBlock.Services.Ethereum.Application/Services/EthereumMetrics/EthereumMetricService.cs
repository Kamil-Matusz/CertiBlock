using CertiBlock.Services.Ethereum.Application.Mappers;
using CertiBlock.Services.Ethereum.Application.RabbitMQ;
using CertiBlock.Services.Ethereum.Application.Services.CoinGecko;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Exceptions;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Enums;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;

public class EthereumMetricService(IEthereumMetricRepository metricsRepository, IEthereumRepository ethereumRepository,
                                   ILogger<EthereumMetricService> logger, IWeb3 web3, ICoinGeckoService coinGeckoService,
                                   MetricPublisher metricPublisher) : IEthereumMetricService
{
    public async Task<Core.Entities.EthereumMetrics> CollectMetricsAsync(Guid certificateId, string transactionHash)
    {
        try
        {
            var txn = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            var latestBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            if (txn == null || receipt == null)
                throw new EthereumTransactionsByHashNotFoundException($"Transaction {transactionHash} not found.");

            var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(receipt.BlockNumber);
            var blockTimestamp = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).UtcDateTime;

            var transaction = await ethereumRepository.GetBlockchainTransactionByCertificateIdAsync(certificateId);
            var submittedAt = transaction?.CreatedAt ?? blockTimestamp;
            var inclusionTimeSeconds = (blockTimestamp - submittedAt).TotalSeconds;

            var dataSizeBytes = string.IsNullOrEmpty(txn.Input) ? 0 : (txn.Input.Length - 2) / 2;
            var confirmations = (int)(latestBlock.Value - receipt.BlockNumber.Value);
            var gasUsed = (long)receipt.GasUsed.Value;
            var effectiveGasPrice = (decimal)receipt.EffectiveGasPrice.Value;
            var transactionCostNative = (gasUsed * effectiveGasPrice) / 1_000_000_000_000_000_000m;

            var assetId = "ethereum";
            var ethUsdPrice = await coinGeckoService.GetPriceUsdAsync(assetId);
            var transactionCostUsd = transactionCostNative * ethUsdPrice;

            var gasUtilizationRatio = txn.Gas.Value > 0
                ? (double)gasUsed / (double)txn.Gas.Value * 100.0
                : 0;
            
            var collectedAt = DateTime.UtcNow;

            var metrics = new Core.Entities.EthereumMetrics
            {
                CertificateId = certificateId,
                Blockchain = Blockchain.Ethereum,
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

            await metricsRepository.SaveEthereumMetricsAsync(metrics);

            var metricEvent = new MetricCollectedEvent(
                metrics.CertificateId,
                Blockchain.Ethereum,
                Operation.Register,
                metrics.GasUsed,
                metrics.InclusionTimeSeconds,
                (double)metrics.TransactionCostNative,
                null,
                collectedAt);
            
            metricPublisher.Publish(metricEvent);

            return metrics;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error collecting metrics for transaction {TransactionHash}", transactionHash);
            throw;
        }
    }

    public async Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId)
    {
        var ethereumTransaction = await metricsRepository.GetTransactionMetricsByCertificateAsync(certificateId);
        if (ethereumTransaction is null)
        {
            throw new EthereumTransactionsNotFoundException(certificateId);
        }

        await metricsRepository.DeleteTransactionMetricsByCertificateIdAsync(certificateId);
    }

    public async Task<EthereumMetricDetailsDto> GetTransactionMetricsByCertificateIdAsync(Guid certificateId)
    {
        var ethereumTransaction = await metricsRepository.GetTransactionMetricsByCertificateAsync(certificateId);

        if (ethereumTransaction is null)
        {
            throw new EthereumTransactionsByCertificateIdNotFoundException(certificateId);
        }

        return EthereumMetricsMapper.Map<EthereumMetricDetailsDto>(ethereumTransaction);
    }
}