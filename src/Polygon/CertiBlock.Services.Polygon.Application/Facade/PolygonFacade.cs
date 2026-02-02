using CertiBlock.Services.Polygon.Application.Services.Polygon;
using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;

namespace CertiBlock.Services.Polygon.Application.Facade;

public class PolygonFacade(IPolygonService polygonService, IPolygonMetricService polygonMetricService) : IPolygonFacade
{
    public async Task DeletePolygonTransactionWithMetricsAsync(Guid certificateId)
    {
        await polygonService.DeletePolygonTransactionByCertificateIdAsync(certificateId);
        await polygonMetricService.DeleteTransactionMetricsByCertificateIdAsync(certificateId);
    }
}