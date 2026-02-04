using CertiBlock.Services.Polygon.Application.Hangfire;
using CertiBlock.Services.Polygon.Application.RabbitMQ;
using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using RabbitMQ.Client;

namespace CertiBlock.Services.Polygon.UnitTests.Schedulers;

public class CheckPolygonFinalizationJobTests
{
    private readonly Mock<IPolygonMetricRepository> _metricRepoMock;
    private readonly Mock<IPolygonRepository> _polygonRepoMock;
    private readonly Mock<IClient> _rpcClientMock;
    private readonly Mock<IModel> _channelMock;
    private readonly CheckPolygonFinalizationJob _job;

    public CheckPolygonFinalizationJobTests()
    {
        _metricRepoMock = new Mock<IPolygonMetricRepository>();
        _polygonRepoMock = new Mock<IPolygonRepository>();
        _rpcClientMock = new Mock<IClient>();

        IWeb3 web3 = new Web3(_rpcClientMock.Object);

        var connectionMock = new Mock<IConnection>();
        _channelMock = new Mock<IModel>();
        var propsMock = new Mock<IBasicProperties>();
        connectionMock.Setup(c => c.CreateModel()).Returns(_channelMock.Object);
        _channelMock.Setup(c => c.CreateBasicProperties()).Returns(propsMock.Object);

        var publisher = new MetricPublisher(
            connectionMock.Object,
            new Mock<ILogger<MetricPublisher>>().Object);

        _job = new CheckPolygonFinalizationJob(
            _metricRepoMock.Object,
            _polygonRepoMock.Object,
            web3,
            publisher,
            new Mock<ILogger<CheckPolygonFinalizationJob>>().Object);
    }

    [Fact]
    public async Task CheckFinalizationAsync_WhenNoUnfinalizedMetrics_ShouldDoNothing()
    {
        _metricRepoMock
            .Setup(x => x.GetUnfinalizedMetricsAsync())
            .ReturnsAsync(Enumerable.Empty<PolygonMetrics>());

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(It.IsAny<PolygonMetrics>()), Times.Never);
        _channelMock.Verify(c => c.BasicPublish(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
            It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Never);
    }

    [Fact]
    public async Task CheckFinalizationAsync_WhenMetricsFinalized_ShouldUpdateAndPublish()
    {
        var collectedAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var certificateId = Guid.NewGuid();
        var metrics = new PolygonMetrics
        {
            CertificateId = certificateId,
            Blockchain = Blockchain.Polygon,
            Operation = Operation.Register,
            BlockNumber = 800,
            GasUsed = 50000,
            InclusionTimeSeconds = 3.2,
            TransactionCostNative = 0.001m,
            IsFinalized = false,
            CollectedAt = collectedAt
        };

        _metricRepoMock
            .Setup(x => x.GetUnfinalizedMetricsAsync())
            .ReturnsAsync(new[] { metrics });

        // Block 1000, threshold = 1000 - 128 = 872, metrics.BlockNumber = 800 < 872 -> finalized
        _rpcClientMock
            .Setup(c => c.SendRequestAsync<HexBigInteger>(
                It.IsAny<RpcRequest>(), It.IsAny<string>()))
            .ReturnsAsync(new HexBigInteger(1000));

        _polygonRepoMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))
            .ReturnsAsync(new BlockchainTransaction
            {
                CertificateId = certificateId,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            });

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(
            It.Is<PolygonMetrics>(m => m.IsFinalized && m.FinalizationTimeSeconds.HasValue)),
            Times.Once);

        _channelMock.Verify(c => c.BasicPublish(
            "", "certiblock.metrics.polygon", false,
            It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckFinalizationAsync_WhenMetricsNotYetFinalized_ShouldNotUpdate()
    {
        var metrics = new PolygonMetrics
        {
            CertificateId = Guid.NewGuid(),
            BlockNumber = 950,
            IsFinalized = false
        };

        _metricRepoMock
            .Setup(x => x.GetUnfinalizedMetricsAsync())
            .ReturnsAsync(new[] { metrics });

        // Block 1000, threshold = 872, metrics.BlockNumber = 950 > 872 -> not finalized
        _rpcClientMock
            .Setup(c => c.SendRequestAsync<HexBigInteger>(
                It.IsAny<RpcRequest>(), It.IsAny<string>()))
            .ReturnsAsync(new HexBigInteger(1000));

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(It.IsAny<PolygonMetrics>()), Times.Never);
        _channelMock.Verify(c => c.BasicPublish(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
            It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Never);
    }

    [Fact]
    public async Task CheckFinalizationAsync_WhenSingleMetricFails_ShouldContinueProcessingOthers()
    {
        var cert1 = Guid.NewGuid();
        var cert2 = Guid.NewGuid();
        var collectedAt = DateTime.UtcNow;

        var metrics1 = new PolygonMetrics
        {
            CertificateId = cert1,
            Blockchain = Blockchain.Polygon,
            Operation = Operation.Register,
            BlockNumber = 700,
            GasUsed = 50000,
            InclusionTimeSeconds = 3.0,
            TransactionCostNative = 0.001m,
            IsFinalized = false,
            CollectedAt = collectedAt
        };

        var metrics2 = new PolygonMetrics
        {
            CertificateId = cert2,
            Blockchain = Blockchain.Polygon,
            Operation = Operation.Register,
            BlockNumber = 750,
            GasUsed = 50000,
            InclusionTimeSeconds = 3.5,
            TransactionCostNative = 0.002m,
            IsFinalized = false,
            CollectedAt = collectedAt
        };

        _metricRepoMock
            .Setup(x => x.GetUnfinalizedMetricsAsync())
            .ReturnsAsync(new[] { metrics1, metrics2 });

        _rpcClientMock
            .Setup(c => c.SendRequestAsync<HexBigInteger>(
                It.IsAny<RpcRequest>(), It.IsAny<string>()))
            .ReturnsAsync(new HexBigInteger(1000));

        _polygonRepoMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(cert1))
            .ThrowsAsync(new Exception("fail"));

        _polygonRepoMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(cert2))
            .ReturnsAsync(new BlockchainTransaction
            {
                CertificateId = cert2,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            });

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(
            It.Is<PolygonMetrics>(m => m.CertificateId == cert2 && m.IsFinalized)),
            Times.Once);
    }
}
