using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;

namespace CertiBlock.Services.Ethereum.Application.Mappers;

public static class EthereumMetricsMapper
{
    public static T Map<T>(EthereumMetrics metrics) where T : EthereumMetricDetailsDto, new() => new T()
    {
        Id = metrics.Id,
        CertificateId = metrics.CertificateId,
        Blockchain = metrics.Blockchain,
        Operation = metrics.Operation,
        TransactionHash = metrics.TransactionHash,
        DataSizeBytes = metrics.DataSizeBytes,
        Confirmations = metrics.Confirmations,
        TransactionCostUsd = metrics.TransactionCostUsd,
        TransactionCostNative = metrics.TransactionCostNative,
        GasUsed = metrics.GasUsed,
        GasUtilizationRatio = metrics.GasUtilizationRatio,
        InclusionTimeSeconds = metrics.InclusionTimeSeconds,
        BlockNumber = metrics.BlockNumber,
        FinalizationTimeSeconds = metrics.FinalizationTimeSeconds,
        IsFinalized = metrics.IsFinalized
    };

    public static IEnumerable<T> MapAll<T>(IEnumerable<EthereumMetrics> metrics) where T : EthereumMetricDetailsDto, new()
    {
        return metrics.Select(metric => Map<T>(metric));
    }
    
    public static EthereumMetricDetailsDto MapToDto(EthereumMetrics metrics) => Map<EthereumMetricDetailsDto>(metrics);

    public static IEnumerable<EthereumMetricDetailsDto> MapAllToDto(IEnumerable<EthereumMetrics> metrics) => 
        MapAll<EthereumMetricDetailsDto>(metrics);
}