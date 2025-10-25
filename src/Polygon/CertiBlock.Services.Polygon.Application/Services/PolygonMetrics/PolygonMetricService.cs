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
    IWeb3 web3, ICoinGeckoService coinGeckoService, MetricPublisher metricPublisher) : IPolygonMetricService
{
    public async Task<Core.Entities.PolygonMetrics> CollectMetricsAsync(Guid certificateId, string transactionHash)
    {
        try
        {
            var txn = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            var latestBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            if (txn == null || receipt == null)
                throw new PolygonTransactionsByHashNotFoundException($"Transaction {transactionHash} not found.");
            
            var dataSizeBytes = string.IsNullOrEmpty(txn.Input) ? 0 : (txn.Input.Length - 2) / 2;
            var confirmations = (int)(latestBlock.Value - receipt.BlockNumber.Value);
            var gasUsed = (long)receipt.GasUsed.Value;
            var effectiveGasPrice = (decimal)receipt.EffectiveGasPrice.Value;
            var transactionCostNative = (gasUsed * effectiveGasPrice) / 1_000_000_000_000_000_000m;
            
            var assetId = "matic-network";
            var maticUsdPrice = await coinGeckoService.GetPriceUsdAsync(assetId);
            var transactionCostUsd = transactionCostNative * maticUsdPrice;
            
            var gasUtilizationRatio = txn.Gas.Value > 0
                ? (double)gasUsed / (double)txn.Gas.Value * 100.0
                : 0;

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
                GasUtilizationRatio = gasUtilizationRatio
            };

            await polygonMetricRepository.SavePolygonMetricsAsync(metrics);
            
            var metricEvent = new MetricCollectedEvent(
                metrics.CertificateId,
                Blockchain.Ethereum,
                Operation.Register,
                metrics.GasUsed,
                1.25,
                (double)metrics.TransactionCostNative,
                DateTime.UtcNow);
            
            metricPublisher.Publish(metricEvent);

            return metrics;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error collecting Polygon metrics for transaction {TransactionHash}", transactionHash);
            throw;
        }
    }

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
}