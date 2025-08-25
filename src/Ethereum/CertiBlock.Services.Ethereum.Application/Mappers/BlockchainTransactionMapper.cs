using CertiBlock.Services.Ethereum.Application.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;

namespace CertiBlock.Services.Ethereum.Application.Mappers;

public static class BlockchainTransactionMapper
{
    public static T Map<T>(BlockchainTransaction transaction) where T : BlockchainTransactionDto, new() => new T()
    {
        Id = transaction.Id,
        CertificateId = transaction.CertificateId,
        CertificateHash = transaction.CertificateHash,
        Blockchain = transaction.Blockchain,
        TransactionHash = transaction.TransactionHash,
        Status = transaction.Status,
        CreatedAt = transaction.CreatedAt,
        ConfirmedAt = transaction.ConfirmedAt,
        FailedAt = transaction.FailedAt
    };

    public static IEnumerable<T> MapAll<T>(IEnumerable<BlockchainTransaction> transactions) where T : BlockchainTransactionDto, new()
    {
        return transactions.Select(transaction => Map<T>(transaction));
    }
    
    public static BlockchainTransactionDto MapToDto(BlockchainTransaction transaction) => Map<BlockchainTransactionDto>(transaction);

    public static IEnumerable<BlockchainTransactionDto> MapAllToDto(IEnumerable<BlockchainTransaction> transactions) => 
        MapAll<BlockchainTransactionDto>(transactions);
}