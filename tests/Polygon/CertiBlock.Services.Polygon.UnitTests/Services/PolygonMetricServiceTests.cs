using CertiBlock.Services.Polygon.Application.RabbitMQ;
using CertiBlock.Services.Polygon.Application.Services.CoinGecko;
using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.Web3;
using RabbitMQ.Client;
using Shouldly;

namespace CertiBlock.Services.Polygon.UnitTests.Services;

public class PolygonMetricServiceTests
{
    private readonly Mock<IPolygonMetricRepository> _metricsRepositoryMock;
    private readonly PolygonMetricService _service;

    public PolygonMetricServiceTests()
    {
        _metricsRepositoryMock = new Mock<IPolygonMetricRepository>();

        var connectionMock = new Mock<IConnection>();
        connectionMock.Setup(x => x.CreateModel()).Returns(new Mock<IModel>().Object);
        var metricPublisher = new MetricPublisher(connectionMock.Object, new Mock<ILogger<MetricPublisher>>().Object);

        _service = new PolygonMetricService(
            _metricsRepositoryMock.Object,
            new Mock<ILogger<PolygonMetricService>>().Object,
            new Mock<IWeb3>().Object,
            new Mock<ICoinGeckoService>().Object,
            metricPublisher,
            new Mock<IPolygonRepository>().Object
        );
    }

    #region GetAllResearchMetricsAsync Tests

    [Fact]
    public async Task GetAllResearchMetricsAsync_ShouldReturnResearchMetrics()
    {
        // Arrange
        var expected = new List<PolygonMetricResearchDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CertificateId = Guid.NewGuid(),
                Blockchain = Blockchain.Polygon,
                Operation = Operation.Register,
                TransactionHash = "0xabc123",
                DataSizeBytes = 4096,
                Confirmations = 128,
                GasUsed = 50000,
                TransactionCostUsd = 0.75m,
                InclusionTimeSeconds = 2.3,
                BlockNumber = 55000000,
                FinalizationTimeSeconds = 256.0
            },
            new()
            {
                Id = Guid.NewGuid(),
                CertificateId = Guid.NewGuid(),
                Blockchain = Blockchain.Polygon,
                Operation = Operation.Register,
                TransactionHash = "0xdef456",
                DataSizeBytes = 2048,
                Confirmations = 64,
                GasUsed = 42000,
                TransactionCostUsd = 0.50m,
                InclusionTimeSeconds = 1.8,
                BlockNumber = 55000001,
                FinalizationTimeSeconds = 260.0
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
            .ReturnsAsync(new List<PolygonMetricResearchDto>());

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
            .ReturnsAsync(new List<PolygonMetricResearchDto>());

        // Act
        await _service.GetAllResearchMetricsAsync();

        // Assert
        _metricsRepositoryMock.Verify(x => x.GetAllResearchMetricsAsync(), Times.Once);
    }

    #endregion
}
