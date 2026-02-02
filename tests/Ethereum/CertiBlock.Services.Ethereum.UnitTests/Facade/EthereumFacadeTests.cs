using CertiBlock.Services.Ethereum.Application.Facade;
using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using Moq;
using Shouldly;

namespace CertiBlock.Services.Ethereum.UnitTests.Facade;

public class EthereumFacadeTests
{
    private readonly Mock<IEthereumService> _ethereumServiceMock;
    private readonly Mock<IEthereumMetricService> _ethereumMetricServiceMock;
    private readonly EthereumFacade _ethereumFacade;

    public EthereumFacadeTests()
    {
        _ethereumServiceMock = new Mock<IEthereumService>();
        _ethereumMetricServiceMock = new Mock<IEthereumMetricService>();
        _ethereumFacade = new EthereumFacade(_ethereumServiceMock.Object, _ethereumMetricServiceMock.Object);
    }

    #region DeleteEthereumTransactionWithMetricsAsync Tests

    [Fact]
    public async Task DeleteEthereumTransactionWithMetricsAsync_ShouldCallBothServices()
    {
        // Arrange
        var certificateId = Guid.NewGuid();

        _ethereumServiceMock
            .Setup(x => x.DeleteEthereumTransactionByCertificateIdAsync(certificateId))
            .Returns(Task.CompletedTask);

        _ethereumMetricServiceMock
            .Setup(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId))
            .Returns(Task.CompletedTask);

        // Act
        await _ethereumFacade.DeleteEthereumTransactionWithMetricsAsync(certificateId);

        // Assert
        _ethereumServiceMock.Verify(x => x.DeleteEthereumTransactionByCertificateIdAsync(certificateId), Times.Once);
        _ethereumMetricServiceMock.Verify(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId), Times.Once);
    }

    [Fact]
    public async Task DeleteEthereumTransactionWithMetricsAsync_WhenEthereumServiceFails_ShouldNotCallMetricService()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var expectedException = new Exception("Ethereum service error");

        _ethereumServiceMock
            .Setup(x => x.DeleteEthereumTransactionByCertificateIdAsync(certificateId))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(
            () => _ethereumFacade.DeleteEthereumTransactionWithMetricsAsync(certificateId));

        exception.Message.ShouldBe("Ethereum service error");
        _ethereumServiceMock.Verify(x => x.DeleteEthereumTransactionByCertificateIdAsync(certificateId), Times.Once);
        _ethereumMetricServiceMock.Verify(x => x.DeleteTransactionMetricsByCertificateIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteEthereumTransactionWithMetricsAsync_WhenMetricServiceFails_ShouldPropagateException()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var expectedException = new Exception("Metric service error");

        _ethereumServiceMock
            .Setup(x => x.DeleteEthereumTransactionByCertificateIdAsync(certificateId))
            .Returns(Task.CompletedTask);

        _ethereumMetricServiceMock
            .Setup(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Should.ThrowAsync<Exception>(
            () => _ethereumFacade.DeleteEthereumTransactionWithMetricsAsync(certificateId));

        exception.Message.ShouldBe("Metric service error");
        _ethereumServiceMock.Verify(x => x.DeleteEthereumTransactionByCertificateIdAsync(certificateId), Times.Once);
        _ethereumMetricServiceMock.Verify(x => x.DeleteTransactionMetricsByCertificateIdAsync(certificateId), Times.Once);
    }

    #endregion
}