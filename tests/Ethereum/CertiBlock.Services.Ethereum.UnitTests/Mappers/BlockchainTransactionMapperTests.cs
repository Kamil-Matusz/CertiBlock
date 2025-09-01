using CertiBlock.Services.Ethereum.Application.Mappers;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Ethereum.UnitTests.Mappers;

public class BlockchainTransactionMapperTests
{
    private BlockchainTransaction CreateSampleTransaction()
    {
        return new BlockchainTransaction
        {
            Id = Guid.Parse("12345678-1234-5678-9012-123456789012"),
            CertificateId = Guid.Parse("87654321-4321-8765-2109-876543210987"),
            CertificateHash = "0xabcdef1234567890abcdef1234567890abcdef12",
            Issuer = "Test Issuer",
            Blockchain = Blockchain.Ethereum,
            TransactionHash = "0x9876543210fedcba9876543210fedcba98765432",
            Status = Status.Confirmed,
            CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc)
        };
    }
    
    [Fact]
    public void Map_WithBlockchainTransactionDto_ShouldMapCorrectly()
    {
        // Arrange
        var transaction = CreateSampleTransaction();

        // Act
        var result = BlockchainTransactionMapper.Map<BlockchainTransactionDto>(transaction);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.CertificateId, result.CertificateId);
        Assert.Equal(transaction.CertificateHash, result.CertificateHash);
        Assert.Equal(transaction.Issuer, result.Issuer);
    }

    [Fact]
    public void Map_WithNullValues_ShouldHandleNullsCorrectly()
    {
        // Arrange
        var transaction = new BlockchainTransaction
        {
            Id = Guid.NewGuid(),
            CertificateId = Guid.NewGuid(),
            CertificateHash = null,
            Issuer = null,
            Status = Status.Submitted
        };

        // Act
        var result = BlockchainTransactionMapper.Map<BlockchainTransactionDto>(transaction);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.CertificateId, result.CertificateId);
        Assert.Null(result.CertificateHash);
        Assert.Null(result.Issuer);
    }
    
    [Fact]
    public void MapAll_WithMultipleTransactions_ShouldMapAllCorrectly()
    {
        // Arrange
        var transactions = new List<BlockchainTransaction>
        {
            CreateSampleTransaction(),
            new BlockchainTransaction
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CertificateId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CertificateHash = "0x1111111111111111111111111111111111111111",
                Issuer = "Another Issuer"
            }
        };

        // Act
        var results = BlockchainTransactionMapper.MapAll<BlockchainTransactionDto>(transactions).ToList();

        // Assert
        Assert.Equal(2, results.Count);
            
        Assert.Equal(transactions[0].Id, results[0].Id);
        Assert.Equal(transactions[0].CertificateId, results[0].CertificateId);
        Assert.Equal(transactions[0].CertificateHash, results[0].CertificateHash);
        Assert.Equal(transactions[0].Issuer, results[0].Issuer);
            
        Assert.Equal(transactions[1].Id, results[1].Id);
        Assert.Equal(transactions[1].CertificateId, results[1].CertificateId);
        Assert.Equal(transactions[1].CertificateHash, results[1].CertificateHash);
        Assert.Equal(transactions[1].Issuer, results[1].Issuer);
    }

    [Fact]
    public void MapAll_WithEmptyCollection_ShouldReturnEmptyCollection()
    {
        // Arrange
        var transactions = new List<BlockchainTransaction>();

        // Act
        var results = BlockchainTransactionMapper.MapAll<BlockchainTransactionDto>(transactions).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void MapToDto_ShouldReturnBlockchainTransactionDto()
    {
        // Arrange
        var transaction = CreateSampleTransaction();

        // Act
        var result = BlockchainTransactionMapper.MapToDto(transaction);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BlockchainTransactionDto>(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.CertificateId, result.CertificateId);
        Assert.Equal(transaction.CertificateHash, result.CertificateHash);
        Assert.Equal(transaction.Issuer, result.Issuer);
    }
    
    [Fact]
    public void MapAllToDto_WithMultipleTransactions_ShouldReturnCorrectDtos()
    {
        // Arrange
        var transactions = new List<BlockchainTransaction>
        {
            CreateSampleTransaction(),
            new BlockchainTransaction
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                CertificateId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                CertificateHash = "0x3333333333333333333333333333333333333333",
                Issuer = "Third Issuer"
            }
        };

        // Act
        var results = BlockchainTransactionMapper.MapAllToDto(transactions).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.All(results, dto => Assert.IsType<BlockchainTransactionDto>(dto));
    }

    [Fact]
    public void MapToResultDto_ShouldMapAllResultDtoProperties()
    {
        // Arrange
        var transaction = CreateSampleTransaction();

        // Act
        var result = BlockchainTransactionMapper.MapToResultDto(transaction);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BlockchainTransactionResultDto>(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(transaction.CertificateId, result.CertificateId);
        Assert.Equal(transaction.TransactionHash, result.TransactionHash);
        Assert.Equal(transaction.Status, result.Status);
        Assert.Equal(transaction.CreatedAt, result.CreatedAt);
    }
    
    [Fact]
    public void MapToResultDto_WithNullTransactionHash_ShouldHandleNullCorrectly()
    {
        // Arrange
        var transaction = CreateSampleTransaction();
        transaction.TransactionHash = null;

        // Act
        var result = BlockchainTransactionMapper.MapToResultDto(transaction);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.TransactionHash);
        Assert.Equal(transaction.Status, result.Status);
    }
    
    [Fact]
    public void MapAllToResultDto_WithMultipleTransactions_ShouldReturnCorrectResultDtos()
    {
        // Arrange
        var transactions = new List<BlockchainTransaction>
        {
            CreateSampleTransaction(),
            new BlockchainTransaction
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                CertificateId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                TransactionHash = "0x5555555555555555555555555555555555555555",
                Status = Status.Failed,
                CreatedAt = new DateTime(2024, 2, 1, 14, 0, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var results = BlockchainTransactionMapper.MapAllToResultDto(transactions).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.All(results, dto => Assert.IsType<BlockchainTransactionResultDto>(dto));
            
        Assert.Equal(transactions[0].TransactionHash, results[0].TransactionHash);
        Assert.Equal(transactions[0].Status, results[0].Status);
        Assert.Equal(transactions[1].TransactionHash, results[1].TransactionHash);
        Assert.Equal(transactions[1].Status, results[1].Status);
    }

    [Fact]
    public void MapAllToResultDto_WithEmptyCollection_ShouldReturnEmptyCollection()
    {
        // Arrange
        var transactions = new List<BlockchainTransaction>();

        // Act
        var results = BlockchainTransactionMapper.MapAllToResultDto(transactions).ToList();

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }
}