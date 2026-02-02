using CertiBlock.Services.Polygon.Core.DTO;

namespace CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;

public interface IPolygonMetricService
{
    Task<Core.Entities.PolygonMetrics> CollectMetricsAsync(Guid certificateId, string transactionHash);
    Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId);
    Task<PolygonMetricDetailsDto> GetTransactionMetricsByCertificateIdAsync(Guid certificateId);
}