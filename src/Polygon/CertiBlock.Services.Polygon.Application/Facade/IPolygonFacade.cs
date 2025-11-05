namespace CertiBlock.Services.Polygon.Application.Facade;

public interface IPolygonFacade
{
    Task DeletePolygonTransactionWithMetricsAsync(Guid certificateId);
}