using CertiBlock.Services.Ethereum.Application.RabbitMQ;
using CertiBlock.Services.Ethereum.Application.Services.CoinGecko;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.Web3;
using RabbitMQ.Client;
using Shouldly;

namespace CertiBlock.Services.Ethereum.UnitTests.Services;

public class EthereumMetricServiceTests
{
    private readonly Mock<IEthereumMetricRepository> _metricsRepositoryMock;
    private readonly EthereumMetricService _service;

    public EthereumMetricServiceTests()
    {
        _metricsRepositoryMock = new Mock<IEthereumMetricRepository>();

        var connectionMock = new Mock<IConnection>();
        connectionMock.Setup(x => x.CreateModel()).Returns(new Mock<IModel>().Object);
        var metricPublisher = new MetricPublisher(connectionMock.Object, new Mock<ILogger<MetricPublisher>>().Object);

        _service = new EthereumMetricService(
            _metricsRepositoryMock.Object,
            new Mock<ILogger<EthereumMetricService>>().Object,
            new Mock<IWeb3>().Object,
            new Mock<ICoinGeckoService>().Object,
            metricPublisher,
            new Mock<IEthereumRepository>().Object
        );
    }

    #region GetAllResearchMetricsAsync Tests

    [Fact]
    public async Task GetAllResearchMetricsAsync_ShouldReturnResearchMetrics()
    {
        // Arrange
        var expected = new List<EthereumMetricResearchDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CertificateId = Guid.NewGuid(),
                Blockchain = Blockchain.Ethereum,
                Operation = Operation.Register,
                TransactionHash = "0xabc123",
                DataSizeBytes = 2048,
                Confirmations = 64,
                GasUsed = 21000,
                TransactionCostUsd = 3.45m,
                InclusionTimeSeconds = 14.5,
                BlockNumber = 19000000,
                FinalizationTimeSeconds = 768.0
            },
            new()
            {
                Id = Guid.NewGuid(),
                CertificateId = Guid.NewGuid(),
                Blockchain = Blockchain.Ethereum,
                Operation = Operation.Register,
                TransactionHash = "0xdef456",
                DataSizeBytes = 1024,
                Confirmations = 32,
                GasUsed = 42000,
                TransactionCostUsd = 5.10m,
                InclusionTimeSeconds = 12.0,
                BlockNumber = 19000001,
                FinalizationTimeSeconds = 800.0
            }
        };

        _metricsRepositoryMock
            .Setup(x => x.GetAllResearchMetricsAsync())
            .ReturnsAsync(expected);

        // Act
        var result = (await _service.GetAllResearchMetricsAsync()).ToList();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].TransactionHash.ShouldBe("0xabc123");
        result[1].TransactionHash.ShouldBe("0xdef456");
    }

    [Fact]
    public async Task GetAllResearchMetricsAsync_WhenNoMetrics_ShouldReturnEmptyCollection()
    {
        // Arrange
        _metricsRepositoryMock
            .Setup(x => x.GetAllResearchMetricsAsync())
            .ReturnsAsync(new List<EthereumMetricResearchDto>());

        // Act
        var result = (await _service.GetAllResearchMetricsAsync()).ToList();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllResearchMetricsAsync_ShouldCallRepositoryOnce()
    {
        // Arrange
        _metricsRepositoryMock
            .Setup(x => x.GetAllResearchMetricsAsync())
            .ReturnsAsync(new List<EthereumMetricResearchDto>());

        // Act
        await _service.GetAllResearchMetricsAsync();

        // Assert
        _metricsRepositoryMock.Verify(x => x.GetAllResearchMetricsAsync(), Times.Once);
    }

    #endregion
}
