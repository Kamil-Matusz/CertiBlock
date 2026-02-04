using CertiBlock.Services.Ethereum.Application.Hangfire;
using CertiBlock.Services.Ethereum.Application.RabbitMQ;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using RabbitMQ.Client;

namespace CertiBlock.Services.Ethereum.UnitTests.Schedulers;

public class CheckEthereumFinalizationJobTests
{
    private readonly Mock<IEthereumMetricRepository> _metricRepoMock;
    private readonly Mock<IEthereumRepository> _ethereumRepoMock;
    private readonly Mock<IClient> _rpcClientMock;
    private readonly Mock<IModel> _channelMock;
    private readonly CheckEthereumFinalizationJob _job;

    public CheckEthereumFinalizationJobTests()
    {
        _metricRepoMock = new Mock<IEthereumMetricRepository>();
        _ethereumRepoMock = new Mock<IEthereumRepository>();
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

        _job = new CheckEthereumFinalizationJob(
            _metricRepoMock.Object,
            _ethereumRepoMock.Object,
            web3,
            publisher,
            new Mock<ILogger<CheckEthereumFinalizationJob>>().Object);
    }

    [Fact]
    public async Task CheckFinalizationAsync_WhenNoUnfinalizedMetrics_ShouldDoNothing()
    {
        _metricRepoMock
            .Setup(x => x.GetUnfinalizedMetricsAsync())
            .ReturnsAsync(Enumerable.Empty<EthereumMetrics>());

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(It.IsAny<EthereumMetrics>()), Times.Never);
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
        var metrics = new EthereumMetrics
        {
            CertificateId = certificateId,
            Blockchain = Blockchain.Ethereum,
            Operation = Operation.Register,
            BlockNumber = 900,
            GasUsed = 21000,
            InclusionTimeSeconds = 15.0,
            TransactionCostNative = 0.001m,
            IsFinalized = false,
            CollectedAt = collectedAt
        };

        _metricRepoMock
            .Setup(x => x.GetUnfinalizedMetricsAsync())
            .ReturnsAsync(new[] { metrics });

        // Block 1000, threshold = 1000 - 64 = 936, metrics.BlockNumber = 900 < 936 -> finalized
        _rpcClientMock
            .Setup(c => c.SendRequestAsync<HexBigInteger>(
                It.IsAny<RpcRequest>(), It.IsAny<string>()))
            .ReturnsAsync(new HexBigInteger(1000));

        _ethereumRepoMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))
            .ReturnsAsync(new BlockchainTransaction
            {
                CertificateId = certificateId,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            });

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(
            It.Is<EthereumMetrics>(m => m.IsFinalized && m.FinalizationTimeSeconds.HasValue)),
            Times.Once);

        _channelMock.Verify(c => c.BasicPublish(
            "", "certiblock.metrics.ethereum", false,
            It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckFinalizationAsync_WhenMetricsNotYetFinalized_ShouldNotUpdate()
    {
        var metrics = new EthereumMetrics
        {
            CertificateId = Guid.NewGuid(),
            BlockNumber = 990,
            IsFinalized = false
        };

        _metricRepoMock
            .Setup(x => x.GetUnfinalizedMetricsAsync())
            .ReturnsAsync(new[] { metrics });

        // Block 1000, threshold = 936, metrics.BlockNumber = 990 > 936 -> not finalized
        _rpcClientMock
            .Setup(c => c.SendRequestAsync<HexBigInteger>(
                It.IsAny<RpcRequest>(), It.IsAny<string>()))
            .ReturnsAsync(new HexBigInteger(1000));

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(It.IsAny<EthereumMetrics>()), Times.Never);
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

        var metrics1 = new EthereumMetrics
        {
            CertificateId = cert1,
            Blockchain = Blockchain.Ethereum,
            Operation = Operation.Register,
            BlockNumber = 800,
            GasUsed = 21000,
            InclusionTimeSeconds = 10.0,
            TransactionCostNative = 0.001m,
            IsFinalized = false,
            CollectedAt = collectedAt
        };

        var metrics2 = new EthereumMetrics
        {
            CertificateId = cert2,
            Blockchain = Blockchain.Ethereum,
            Operation = Operation.Register,
            BlockNumber = 850,
            GasUsed = 21000,
            InclusionTimeSeconds = 12.0,
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

        _ethereumRepoMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(cert1))
            .ThrowsAsync(new Exception("fail"));

        _ethereumRepoMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(cert2))
            .ReturnsAsync(new BlockchainTransaction
            {
                CertificateId = cert2,
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            });

        await _job.CheckFinalizationAsync();

        _metricRepoMock.Verify(x => x.UpdateMetricsAsync(
            It.Is<EthereumMetrics>(m => m.CertificateId == cert2 && m.IsFinalized)),
            Times.Once);
    }
}
