using CertiBlock.Services.Ethereum.Application.Services;
using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Ethereum;
using CertiBlock.Services.Ethereum.Core.Exceptions;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Shouldly;

namespace CertiBlock.Services.Ethereum.UnitTests.Services;

public class EthereumServiceTests
{
    private readonly Mock<IEthereumRepository> _ethereumRepositoryMock;
    private readonly Mock<ILogger<EthereumService>> _loggerMock;
    private readonly Mock<IWeb3> _web3Mock;
    private readonly EthereumService _ethereumService;

    public EthereumServiceTests()
    {
        _ethereumRepositoryMock = new Mock<IEthereumRepository>();
        _loggerMock = new Mock<ILogger<EthereumService>>();
        _web3Mock = new Mock<IWeb3>();
        var ethereumOptions = new EthereumOptions
        {
            PrivateKey = "0x123456789abcdef123456789abcdef123456789abcdef123456789abcdef1234",
            InfuraUrl = "https://mainnet.infura.io/v3/test-key"
        };
        

        _ethereumService = new EthereumService(
            _ethereumRepositoryMock.Object,
            _loggerMock.Object,
            _web3Mock.Object,
            ethereumOptions
        );
    }
    
    #region GetEthereumTransactionByIdAsync Tests
    
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

        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))
            .ReturnsAsync(transaction);

        // Act
        var result = await _ethereumService.GetEthereumTransactionByIdAsync(id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
    }

    [Fact]
    public async Task GetEthereumTransactionByIdAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))!
            .ReturnsAsync((BlockchainTransaction)null!);

        // Act & Assert
        await Should.ThrowAsync<EthereumTransactionsNotFoundException>(() => 
            _ethereumService.GetEthereumTransactionByIdAsync(id));
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

        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))
            .ReturnsAsync(transaction);

        // Act
        var result = await _ethereumService.GetBlockchainTransactionByCertificateIdAsync(certificateId);

        // Assert
        result.ShouldNotBeNull();
        result.CertificateId.ShouldBe(certificateId);
    }

    [Fact]
    public async Task GetBlockchainTransactionByCertificateIdAsync_WithNonExistingCertificateId_ShouldThrowNotFoundException()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))!
            .ReturnsAsync((BlockchainTransaction)null!);

        // Act & Assert
        await Should.ThrowAsync<EthereumTransactionsByCertificateIdNotFoundException>(
            () => _ethereumService.GetBlockchainTransactionByCertificateIdAsync(certificateId));
    }

    #endregion
    
    #region DeleteEthereumTransactionAsync Tests

    [Fact]
    public async Task DeleteEthereumTransactionAsync_WithExistingTransaction_ShouldDeleteTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        var transaction = new BlockchainTransaction { Id = id };

        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))
            .ReturnsAsync(transaction);

        _ethereumRepositoryMock
            .Setup(x => x.DeleteBlockchainTransactionAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        await _ethereumService.DeleteEthereumTransactionAsync(id);

        // Assert
        _ethereumRepositoryMock.Verify(x => x.DeleteBlockchainTransactionAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteEthereumTransactionAsync_WithNonExistingTransaction_ShouldThrowNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByIdAsync(id))!
            .ReturnsAsync((BlockchainTransaction)null!);

        // Act & Assert
        await Should.ThrowAsync<EthereumTransactionsNotFoundException>(
            () => _ethereumService.DeleteEthereumTransactionAsync(id));

        _ethereumRepositoryMock.Verify(x => x.DeleteBlockchainTransactionAsync(It.IsAny<Guid>()), Times.Never);
    }

    #endregion
    
    #region GetEthereumTransactionsByStatusAsync Tests

    [Fact]
    public async Task GetEthereumTransactionsByStatusAsync_WithSingleStatus_ShouldReturnFilteredTransactions()
    {
        // Arrange
        var status = Status.Confirmed;
        var transactions = new List<BlockchainTransaction>
        {
            new() { Id = Guid.NewGuid(), Status = Status.Confirmed },
            new() { Id = Guid.NewGuid(), Status = Status.Confirmed }
        };

        _ethereumRepositoryMock
            .Setup(x => x.GetTransactionsByStatusAsync(status))
            .ReturnsAsync(transactions);

        // Act
        var result = await _ethereumService.GetEthereumTransactionsByStatusAsync(status);

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

        _ethereumRepositoryMock
            .Setup(x => x.GetTransactionsByStatusAsync(statuses))
            .ReturnsAsync(transactions);

        // Act
        var result = await _ethereumService.GetEthereumTransactionsByStatusAsync(statuses);

        // Assert
        var blockchainTransactionDtos = result.ToList();
        blockchainTransactionDtos.ShouldNotBeNull();
        blockchainTransactionDtos.Count().ShouldBe(2);
    }

    #endregion
    
    #region GetEthereumTransactionsPagedAsync Tests

    [Fact]
    public async Task GetEthereumTransactionsPagedAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var page = 1;
        var pageSize = 10;
        var transactions = new List<BlockchainTransaction>
        {
            new() { Id = Guid.NewGuid(), Status = Status.Confirmed },
            new() { Id = Guid.NewGuid(), Status = Status.Submitted }
        };

        _ethereumRepositoryMock
            .Setup(x => x.GetTransactionsPagedAsync(page, pageSize))
            .ReturnsAsync(transactions);

        // Act
        var result = await _ethereumService.GetEthereumTransactionsPagedAsync(page, pageSize);

        // Assert
        var blockchainTransactionResultDtos = result.ToList();
        blockchainTransactionResultDtos.ShouldNotBeNull();
        blockchainTransactionResultDtos.Count().ShouldBe(2);
    }

    #endregion
    
    #region GetEthereumTransactionCountAsync Tests

    [Fact]
    public async Task GetEthereumTransactionCountAsync_ShouldReturnCount()
    {
        // Arrange
        var expectedCount = 100L;
        _ethereumRepositoryMock
            .Setup(x => x.GetTransactionCountAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _ethereumService.GetEthereumTransactionCountAsync();

        // Assert
        result.ShouldBe(expectedCount);
    }

    #endregion
    
    #region DeleteEthereumTransactionByCertificateIdAsync Tests

    [Fact]
    public async Task DeleteEthereumTransactionByCertificateIdAsync_WithExistingTransaction_ShouldDeleteTransaction()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        var transaction = new BlockchainTransaction 
        { 
            Id = Guid.NewGuid(),
            CertificateId = certificateId 
        };

        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))
            .ReturnsAsync(transaction);

        _ethereumRepositoryMock
            .Setup(x => x.DeleteBlockchainTransactionByCertificateIdAsync(certificateId))
            .Returns(Task.CompletedTask);

        // Act
        await _ethereumService.DeleteEthereumTransactionByCertificateIdAsync(certificateId);

        // Assert
        _ethereumRepositoryMock.Verify(x => x.DeleteBlockchainTransactionByCertificateIdAsync(certificateId), Times.Once);
    }

    [Fact]
    public async Task DeleteEthereumTransactionByCertificateIdAsync_WithNonExistingTransaction_ShouldThrowNotFoundException()
    {
        // Arrange
        var certificateId = Guid.NewGuid();
        _ethereumRepositoryMock
            .Setup(x => x.GetBlockchainTransactionByCertificateIdAsync(certificateId))!
            .ReturnsAsync((BlockchainTransaction)null!);

        // Act & Assert
        await Should.ThrowAsync<EthereumTransactionsNotFoundException>(
            () => _ethereumService.DeleteEthereumTransactionByCertificateIdAsync(certificateId));

        _ethereumRepositoryMock.Verify(x => x.DeleteBlockchainTransactionByCertificateIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    #endregion
}
