using CertiBlock.Services.Polygon.Application.Services;
using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Services.Polygon.Core.Exceptions;
using CertiBlock.Services.Polygon.Core.Polygon;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.Web3;
using Shouldly;

namespace CertiBlock.Services.Polygon.UnitTests.Services;

public class PolygonServiceTests
{
    private readonly Mock<IPolygonRepository> _polygonRepositoryMock;
    private readonly Mock<ILogger<PolygonService>> _loggerMock;
    private readonly Mock<IWeb3> _web3Mock;
    private readonly PolygonService _polygonService;

    public PolygonServiceTests()
    {
        _polygonRepositoryMock = new Mock<IPolygonRepository>();
        _loggerMock = new Mock<ILogger<PolygonService>>();
        _web3Mock = new Mock<IWeb3>();
        var polygonOptions = new PolygonOptions()
        {
            PrivateKey = "0x123456789abcdef123456789abcdef123456789abcdef123456789abcdef1234",
            InfuraUrl = "https://mainnet.infura.io/v3/test-key"
        };
        

        _polygonService = new PolygonService(
            _polygonRepositoryMock.Object,
            _loggerMock.Object,
            _web3Mock.Object,
            polygonOptions
        );
    }
    
    #region GetPolygonTransactionByIdAsync Tests
    
    [Fact]
    public async Task GetEthereumTransactionByIdAsync_WithExistingId_ShouldReturnTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        var transaction = new BlockchainTransaction
        {
            Id = id,
            CertificateId = Guid.NewGuid(),
            Status = Status.Confirmed
        };

        _polygonRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))
            .ReturnsAsync(transaction);

        // Act
        var result = await _polygonService.GetPolygonTransactionByIdAsync(id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
    }

    [Fact]
    public async Task GetEthereumTransactionByIdAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _polygonRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))!
            .ReturnsAsync((BlockchainTransaction)null!);

        // Act & Assert
        await Should.ThrowAsync<PolygonTransactionsNotFoundException>(() => 
            _polygonService.GetPolygonTransactionByIdAsync(id));
    }
    
    #endregion
    
    #region GetBlockchainTransactionByCertificateIdAsync Tests

    [Fact]
    public async Task GetBlockchainTransactionByCertificateIdAsync_WithExistingCertificateId_ShouldReturnTransaction()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var transaction = new BlockchainTransaction
        {
            Id = Guid.NewGuid(),
            CertificateId = certificateId,
            Status = Status.Confirmed
        };

        _polygonRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))
            .ReturnsAsync(transaction);

        // Act
        var result = await _polygonService.GetBlockchainTransactionByCertificateIdAsync(certificateId);

        // Assert
        result.ShouldNotBeNull();
        result.CertificateId.ShouldBe(certificateId);
    }

    [Fact]
    public async Task GetBlockchainTransactionByCertificateIdAsync_WithNonExistingCertificateId_ShouldThrowNotFoundException()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        _polygonRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))!
            .ReturnsAsync((BlockchainTransaction)null!);

        // Act & Assert
        await Should.ThrowAsync<PolygonTransactionsByCertificateIdNotFoundException>(
            () => _polygonService.GetBlockchainTransactionByCertificateIdAsync(certificateId));
    }

    #endregion
    
    #region DeletePolygonTransactionAsync Tests

    [Fact]
    public async Task DeletePolygonTransactionAsync_WithExistingTransaction_ShouldDeleteTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        var transaction = new BlockchainTransaction { Id = id };

        _polygonRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))
            .ReturnsAsync(transaction);

        _polygonRepositoryMock
            .Setup(x => x.DeleteBlockchainTransactionAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        await _polygonService.DeletePolygonTransactionAsync(id);

        // Assert
        _polygonRepositoryMock.Verify(x => x.DeleteBlockchainTransactionAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteEthereumTransactionAsync_WithNonExistingTransaction_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _polygonRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))!
            .ReturnsAsync((BlockchainTransaction)null!);

        // Act & Assert
        await Should.ThrowAsync<PolygonTransactionsNotFoundException>(
            () => _polygonService.DeletePolygonTransactionAsync(id));

        _polygonRepositoryMock.Verify(x => x.DeleteBlockchainTransactionAsync(It.IsAny<Guid>()), Times.Never);
    }

    #endregion
    
    #region GetPolygonTransactionsByStatusAsync Tests

    [Fact]
    public async Task GetPolygonTransactionsByStatusAsync_WithSingleStatus_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var status = Status.Confirmed;
        var transactions = new List<BlockchainTransaction>
        {
            new() { Id = Guid.NewGuid(), Status = Status.Confirmed },
            new() { Id = Guid.NewGuid(), Status = Status.Confirmed }
        };

        _polygonRepositoryMock
            .Setup(x => x.GetTransactionsByStatusAsync(status))
            .ReturnsAsync(transactions);

        // Act
        var result = await _polygonService.GetPolygonTransactionsByStatusAsync(status);

        // Assert
        var blockchainTransactionDtos = result.ToList();
        blockchainTransactionDtos.ShouldNotBeNull();
        blockchainTransactionDtos.Count().ShouldBe(2);
    }

    [Fact]
    public async Task GetEthereumTransactionsByStatusAsync_WithMultipleStatuses_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var statuses = new[] { Status.Confirmed, Status.Submitted };
        var transactions = new List<BlockchainTransaction>
        {
            new() { Id = Guid.NewGuid(), Status = Status.Confirmed },
            new() { Id = Guid.NewGuid(), Status = Status.Submitted }
        };

        _polygonRepositoryMock
            .Setup(x => x.GetTransactionsByStatusAsync(statuses))
            .ReturnsAsync(transactions);

        // Act
        var result = await _polygonService.GetPolygonTransactionsByStatusAsync(statuses);

        // Assert
        var blockchainTransactionDtos = result.ToList();
        blockchainTransactionDtos.ShouldNotBeNull();
        blockchainTransactionDtos.Count().ShouldBe(2);
    }

    #endregion
    
    #region GetPolygonTransactionsPagedAsync Tests

    [Fact]
    public async Task GetPolygonTransactionsPagedAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var transactions = new List<BlockchainTransaction>
        {
            new() { Id = Guid.NewGuid(), Status = Status.Confirmed },
            new() { Id = Guid.NewGuid(), Status = Status.Submitted }
        };

        _polygonRepositoryMock
            .Setup(x => x.GetTransactionsPagedAsync(page, pageSize))
            .ReturnsAsync(transactions);

        // Act
        var result = await _polygonService.GetPolygonTransactionsPagedAsync(page, pageSize);

        // Assert
        var blockchainTransactionResultDtos = result.ToList();
        blockchainTransactionResultDtos.ShouldNotBeNull();
        blockchainTransactionResultDtos.Count().ShouldBe(2);
    }

    #endregion
    
    #region GetPolygonTransactionCountAsync Tests

    [Fact]
    public async Task GetPolygonTransactionCountAsync_ShouldReturnCount()
    {
        // Arrange
        var expectedCount = 100L;
        _polygonRepositoryMock
            .Setup(x => x.GetTransactionCountAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _polygonService.GetPolygonTransactionCountAsync();

        // Assert
        result.ShouldBe(expectedCount);
    }

    #endregion
}
