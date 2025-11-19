using CertiBlock.Services.Polygon.Application.Hangfire;
using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Services.Polygon.Core.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace CertiBlock.Services.Polygon.UnitTests.Schedulers;

public class FetchMissingPolygonMetricsJobTests
{
    private readonly Mock<IPolygonRepository> _polygonRepositoryMock;
    private readonly Mock<IPolygonMetricRepository> _polygonMetricRepositoryMock;
    private readonly Mock<IPolygonMetricService> _polygonMetricServiceMock;

    private readonly FetchMissingPolygonMetricsJob _job;

    public FetchMissingPolygonMetricsJobTests()
    {
        _polygonRepositoryMock = new Mock<IPolygonRepository>();
        _polygonMetricRepositoryMock = new Mock<IPolygonMetricRepository>();
        _polygonMetricServiceMock = new Mock<IPolygonMetricService>();
        var loggerMock = new Mock<ILogger<FetchMissingPolygonMetricsJob>>();

        _job = new FetchMissingPolygonMetricsJob(
            _polygonRepositoryMock.Object,
            _polygonMetricRepositoryMock.Object,
            _polygonMetricServiceMock.Object,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task FetchMissingMetricsAsync_WhenAllCertificatesHaveMetrics_ShouldDoNothing()
    {
        // Arrange
        var certificateTransactions = new Dictionary<Guid, string>
        {
            { Guid.NewGuid(), "0xabc123" }
        };

        _polygonRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(certificateTransactions);

        _polygonMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(certificateTransactions.Keys);

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _polygonMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(It.IsAny<Guid>(), It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task FetchMissingMetricsAsync_WhenMissingMetricsExist_ShouldInvokeMetricCollection()
    {
        // Arrange
        var missingId = Guid.NewGuid();

        var certificateTransactions = new Dictionary<Guid, string>
        {
            { missingId, "0xmissing" },
            { Guid.NewGuid(), "0xexists" }
        };

        _polygonRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(certificateTransactions);

        _polygonMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(new[] { certificateTransactions.Keys.Last() });

        _polygonMetricServiceMock
            .Setup(x => x.CollectMetricsAsync(missingId, "0xmissing"))
            .ReturnsAsync(new PolygonMetrics());

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _polygonMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(missingId, "0xmissing"),
            Times.Once
        );
    }

    [Fact]
    public async Task FetchMissingMetricsAsync_WhenCollectMetricsThrows_ShouldContinueProcessingOthers()
    {
        // Arrange
        var cert1 = Guid.NewGuid();
        var cert2 = Guid.NewGuid();

        var data = new Dictionary<Guid, string>
        {
            { cert1, "0x1" },
            { cert2, "0x2" }
        };

        _polygonRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(data);

        _polygonMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(Array.Empty<Guid>());

        _polygonMetricServiceMock
            .SetupSequence(x => x.CollectMetricsAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("fail"))
            .ReturnsAsync(new PolygonMetrics());

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _polygonMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(cert1, "0x1"),
            Times.Once
        );

        _polygonMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(cert2, "0x2"),
            Times.Once
        );
    }

    [Fact]
    public async Task FetchMissingMetricsAsync_ShouldCollectMetricsForExistingCertificate()
    {
        // Arrange
        var cert = Guid.NewGuid();
        var tx = "0xtx";

        _polygonRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(new Dictionary<Guid, string> { { cert, tx } });

        _polygonMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(Array.Empty<Guid>());

        _polygonMetricServiceMock
            .Setup(x => x.CollectMetricsAsync(cert, tx))
            .ReturnsAsync(new PolygonMetrics());

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _polygonMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(cert, tx),
            Times.Once
        );
    }
}
