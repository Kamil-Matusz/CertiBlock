using CertiBlock.Services.Ethereum.Application.Mappers;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Ethereum.UnitTests.Mappers;

public class EthereumMetricsMapperTests
{
    private EthereumMetrics CreateSampleMetrics()
    {
        return new EthereumMetrics
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CertificateId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Blockchain = Blockchain.Ethereum,
            Operation = Operation.Register,
            TransactionHash = "0xabcdef1234567890abcdef1234567890abcdef12",
            DataSizeBytes = 2048,
            Confirmations = 12,
            TransactionCostUsd = 3.45m,
            TransactionCostNative = 0.0021m,
            GasUsed = 21000,
            GasUtilizationRatio = 0.95,
            InclusionTimeSeconds = 15.5,
            BlockNumber = 1234567,
            FinalizationTimeSeconds = 120.5,
            IsFinalized = true,
            CollectedAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)
        };
    }

    [Fact]
    public void Map_WithEthereumMetricDetailsDto_ShouldMapCorrectly()
    {
        // Arrange
        var metrics = CreateSampleMetrics();

        // Act
        var result = EthereumMetricsMapper.Map<EthereumMetricDetailsDto>(metrics);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(metrics.Id, result.Id);
        Assert.Equal(metrics.CertificateId, result.CertificateId);
        Assert.Equal(metrics.Blockchain, result.Blockchain);
        Assert.Equal(metrics.Operation, result.Operation);
        Assert.Equal(metrics.TransactionHash, result.TransactionHash);
        Assert.Equal(metrics.DataSizeBytes, result.DataSizeBytes);
        Assert.Equal(metrics.Confirmations, result.Confirmations);
        Assert.Equal(metrics.TransactionCostUsd, result.TransactionCostUsd);
        Assert.Equal(metrics.TransactionCostNative, result.TransactionCostNative);
        Assert.Equal(metrics.GasUsed, result.GasUsed);
        Assert.Equal(metrics.GasUtilizationRatio, result.GasUtilizationRatio);
        Assert.Equal(metrics.InclusionTimeSeconds, result.InclusionTimeSeconds);
        Assert.Equal(metrics.BlockNumber, result.BlockNumber);
        Assert.Equal(metrics.FinalizationTimeSeconds, result.FinalizationTimeSeconds);
        Assert.Equal(metrics.IsFinalized, result.IsFinalized);
    }

    [Fact]
    public void Map_WithNullValues_ShouldHandleNullsCorrectly()
    {
        // Arrange
        var metrics = new EthereumMetrics
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            CertificateId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Blockchain = Blockchain.Ethereum,
            Operation = Operation.Register,
            TransactionHash = null
        };

        // Act
        var result = EthereumMetricsMapper.Map<EthereumMetricDetailsDto>(metrics);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(metrics.Id, result.Id);
        Assert.Equal(metrics.CertificateId, result.CertificateId);
        Assert.Equal(metrics.Blockchain, result.Blockchain);
        Assert.Equal(metrics.Operation, result.Operation);
        Assert.Null(result.TransactionHash);
    }

    [Fact]
    public void MapAll_WithMultipleMetrics_ShouldMapAllCorrectly()
    {
        // Arrange
        var metricsList = new List<EthereumMetrics>
        {
            CreateSampleMetrics(),
            new EthereumMetrics
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                CertificateId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Blockchain = Blockchain.Ethereum,
                Operation = Operation.Register,
                TransactionHash = "0x1111111111111111111111111111111111111111"
            }
        };

        // Act
        var results = EthereumMetricsMapper.MapAll<EthereumMetricDetailsDto>(metricsList).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal(metricsList[0].Id, results[0].Id);
        Assert.Equal(metricsList[1].Id, results[1].Id);
    }

    [Fact]
    public void MapAll_WithEmptyCollection_ShouldReturnEmptyCollection()
    {
        // Arrange
        var metricsList = new List<EthereumMetrics>();

        // Act
        var results = EthereumMetricsMapper.MapAll<EthereumMetricDetailsDto>(metricsList).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void MapToDto_ShouldReturnEthereumMetricDetailsDto()
    {
        // Arrange
        var metrics = CreateSampleMetrics();

        // Act
        var result = EthereumMetricsMapper.MapToDto(metrics);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EthereumMetricDetailsDto>(result);
        Assert.Equal(metrics.Id, result.Id);
        Assert.Equal(metrics.CertificateId, result.CertificateId);
    }

    [Fact]
    public void MapAllToDto_WithMultipleMetrics_ShouldReturnCorrectDtos()
    {
        // Arrange
        var metricsList = new List<EthereumMetrics>
        {
            CreateSampleMetrics(),
            new EthereumMetrics
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                CertificateId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Blockchain = Blockchain.Ethereum,
                Operation = Operation.Register
            }
        };

        // Act
        var results = EthereumMetricsMapper.MapAllToDto(metricsList).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.All(results, dto => Assert.IsType<EthereumMetricDetailsDto>(dto));
    }

    [Fact]
    public void MapAllToDto_WithEmptyCollection_ShouldReturnEmptyCollection()
    {
        // Arrange
        var metricsList = new List<EthereumMetrics>();

        // Act
        var results = EthereumMetricsMapper.MapAllToDto(metricsList).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }
}