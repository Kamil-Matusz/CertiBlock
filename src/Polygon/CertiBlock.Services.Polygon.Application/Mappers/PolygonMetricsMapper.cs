using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Services.Polygon.Core.Entities;

namespace CertiBlock.Services.Polygon.Application.Mappers;

public static class PolygonMetricsMapper
{
    public static T Map<T>(PolygonMetrics metrics) where T : PolygonMetricDetailsDto, new() => new T()
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
        InclusionTimeSeconds = metrics.InclusionTimeSeconds
    };
    
    public static IEnumerable<T> MapAll<T>(IEnumerable<PolygonMetrics> metrics) where T : PolygonMetricDetailsDto, new()
    {
        return metrics.Select(metric => Map<T>(metric));
    }
    
    public static PolygonMetricDetailsDto MapToDto(PolygonMetrics metrics) => Map<PolygonMetricDetailsDto>(metrics);

    public static IEnumerable<PolygonMetricDetailsDto> MapAllToDto(IEnumerable<PolygonMetrics> metrics) => 
        MapAll<PolygonMetricDetailsDto>(metrics);
}