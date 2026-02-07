using CertiBlock.Services.Polygon.Core.Entities;

namespace CertiBlock.Services.Polygon.Core.Repositories;

public interface IPolygonMetricRepository
{
    Task SavePolygonMetricsAsync(PolygonMetrics polygonMetrics);
    Task<PolygonMetrics> GetTransactionMetricsByCertificateAsync(Guid certificateId);
    Task DeleteTransactionMetricsByCertificateIdAsync(Guid certificateId);
    Task<IEnumerable<Guid>> GetAllCertificateIdsWithMetricsAsync();
    Task<IEnumerable<PolygonMetrics>> GetUnfinalizedMetricsAsync();
    Task UpdatePolygonMetricsAsync(PolygonMetrics metrics);
}