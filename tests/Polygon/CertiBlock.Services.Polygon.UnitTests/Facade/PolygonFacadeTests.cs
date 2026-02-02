using CertiBlock.Services.Polygon.Application.Facade;
using CertiBlock.Services.Polygon.Application.Services.Polygon;
using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using Moq;
using Shouldly;

namespace CertiBlock.Services.Polygon.UnitTests.Facade;

public class PolygonFacadeTests
{
    private readonly Mock<IPolygonService> _polygonServiceMock;
    private readonly Mock<IPolygonMetricService> _polygonMetricServiceMock;
    private readonly PolygonFacade _polygonFacade;

    public PolygonFacadeTests()
    {
        _polygonServiceMock = new Mock<IPolygonService>();
        _polygonMetricServiceMock = new Mock<IPolygonMetricService>();
        _polygonFacade = new PolygonFacade(_polygonServiceMock.Object, _polygonMetricServiceMock.Object);
    }

    #region DeletePolygonTransactionWithMetricsAsync Tests

    [Fact]
    public async Task DeletePolygonTransactionWithMetricsAsync_ShouldCallBothServices()
    {
        // Arrange
        var certificateId = Guid.NewGuid();

        _polygonServiceMock
            .Setup(x => x.DeletePolygonTransactionByCertificateIdAsync(certificateId))
            .Returns(Task.CompletedTask);

        _polygonMetricServiceMock
            .Setup(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId))
            .Returns(Task.CompletedTask);

        // Act
        await _polygonFacade.DeletePolygonTransactionWithMetricsAsync(certificateId);

        // Assert
        _polygonServiceMock.Verify(x => x.DeletePolygonTransactionByCertificateIdAsync(certificateId), Times.Once);
        _polygonMetricServiceMock.Verify(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId), Times.Once);
    }

    [Fact]
    public async Task DeletePolygonTransactionWithMetricsAsync_WhenPolygonServiceFails_ShouldNotCallMetricService()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var expectedException = new Exception("Polygon service error");

        _polygonServiceMock
            .Setup(x => x.DeletePolygonTransactionByCertificateIdAsync(certificateId))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(
            () => _polygonFacade.DeletePolygonTransactionWithMetricsAsync(certificateId));

        exception.Message.ShouldBe("Polygon service error");
        _polygonServiceMock.Verify(x => x.DeletePolygonTransactionByCertificateIdAsync(certificateId), Times.Once);
        _polygonMetricServiceMock.Verify(x => x.DeleteTransactionMetricsByCertificateIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeletePolygonTransactionWithMetricsAsync_WhenMetricServiceFails_ShouldPropagateException()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var expectedException = new Exception("Metric service error");

        _polygonServiceMock
            .Setup(x => x.DeletePolygonTransactionByCertificateIdAsync(certificateId))
            .Returns(Task.CompletedTask);

        _polygonMetricServiceMock
            .Setup(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(
            () => _polygonFacade.DeletePolygonTransactionWithMetricsAsync(certificateId));

        exception.Message.ShouldBe("Metric service error");
        _polygonServiceMock.Verify(x => x.DeletePolygonTransactionByCertificateIdAsync(certificateId), Times.Once);
        _polygonMetricServiceMock.Verify(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId), Times.Once);
    }

    #endregion
}