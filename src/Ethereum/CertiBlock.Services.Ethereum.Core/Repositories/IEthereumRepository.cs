using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Ethereum.Core.Repositories;

public interface IEthereumRepository
{
    Task<BlockchainTransaction> GetBlockchainTransactionByIdAsync(Guid id);
    Task<BlockchainTransaction> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId);
    Task<IEnumerable<BlockchainTransaction>> GetAllBlockchainTransactionsAsync();
    Task SaveBlockchainTransactionAsync(BlockchainTransaction blockchainTransaction);
    Task DeleteBlockchainTransactionAsync(Guid id);
    Task UpdateBlockchainTransactionAsync(BlockchainTransaction transaction);
    Task<IEnumerable<BlockchainTransaction>> GetTransactionsPagedAsync(int page, int pageSize);
    Task<IEnumerable<BlockchainTransaction>> GetTransactionsByStatusAsync(Status status);
    Task<IEnumerable<BlockchainTransaction>> GetTransactionsByStatusAsync(params Status[] statuses);
    Task<long> GetTransactionCountAsync();
    Task<BlockchainTransaction?> GetByTransactionHashAsync(string transactionHash);
    Task DeleteBlockchainTransactionByCertificateIdAsync(Guid certificateId);
}