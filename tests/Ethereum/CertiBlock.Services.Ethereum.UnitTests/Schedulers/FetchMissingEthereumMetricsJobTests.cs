using CertiBlock.Services.Ethereum.Application.Hangfire;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace CertiBlock.Services.Ethereum.UnitTests.Schedulers;

public class FetchMissingEthereumMetricsJobTests
{
    private readonly Mock<IEthereumRepository> _ethereumRepositoryMock;
    private readonly Mock<IEthereumMetricRepository> _ethereumMetricRepositoryMock;
    private readonly Mock<IEthereumMetricService> _ethereumMetricServiceMock;

    private readonly FetchMissingEthereumMetricsJob _job;

    public FetchMissingEthereumMetricsJobTests()
    {
        _ethereumRepositoryMock = new Mock<IEthereumRepository>();
        _ethereumMetricRepositoryMock = new Mock<IEthereumMetricRepository>();
        _ethereumMetricServiceMock = new Mock<IEthereumMetricService>();
        var loggerMock = new Mock<ILogger<FetchMissingEthereumMetricsJob>>();

        _job = new FetchMissingEthereumMetricsJob(
            _ethereumRepositoryMock.Object,
            _ethereumMetricRepositoryMock.Object,
            _ethereumMetricServiceMock.Object,
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

        _ethereumRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(certificateTransactions);
        
        _ethereumMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(certificateTransactions.Keys);

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _ethereumMetricServiceMock.Verify(
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

        _ethereumRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(certificateTransactions);
        
        _ethereumMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(new[] { certificateTransactions.Keys.Last() });

        _ethereumMetricServiceMock
            .Setup(x => x.CollectMetricsAsync(missingId, "0xmissing"))
            .ReturnsAsync(new EthereumMetrics());

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _ethereumMetricServiceMock.Verify(
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

        _ethereumRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(data);

        _ethereumMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(Array.Empty<Guid>());

        _ethereumMetricServiceMock
            .SetupSequence(x => x.CollectMetricsAsync(It.IsAny<Guid>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("fail"))
            .ReturnsAsync(new EthereumMetrics());

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _ethereumMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(cert1, "0x1"),
            Times.Once);

        _ethereumMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(cert2, "0x2"),
            Times.Once);
    }

    [Fact]
    public async Task FetchMissingMetricsAsync_ShouldCollectMetricsForExistingCertificate()
    {
        // Arrange
        var cert = Guid.NewGuid();
        var tx = "0xtx";

        _ethereumRepositoryMock
            .Setup(x => x.GetConfirmedCertificateTransactionsAsync())
            .ReturnsAsync(new Dictionary<Guid, string> { { cert, tx } });

        _ethereumMetricRepositoryMock
            .Setup(x => x.GetAllCertificateIdsWithMetricsAsync())
            .ReturnsAsync(Array.Empty<Guid>());

        _ethereumMetricServiceMock
            .Setup(x => x.CollectMetricsAsync(cert, tx))
            .ReturnsAsync(new EthereumMetrics());

        // Act
        await _job.FetchMissingMetricsAsync();

        // Assert
        _ethereumMetricServiceMock.Verify(
            x => x.CollectMetricsAsync(cert, tx),
            Times.Once);
    }
}