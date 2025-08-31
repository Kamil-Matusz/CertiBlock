using CertiBlock.Services.Ethereum.Core.Entities;

namespace CertiBlock.Services.Ethereum.Core.Repositories;

public interface IEthereumRepository
{
    Task<BlockchainTransaction> GetBlockchainTransactionByIdAsync(Guid id);
    Task<IEnumerable<BlockchainTransaction>> GetAllBlockchainTransactionsAsync();
    Task SaveBlockchainTransactionAsync(BlockchainTransaction blockchainTransaction);
    Task DeleteBlockchainTransactionAsync(Guid id);
    Task UpdateBlockchainTransactionAsync(BlockchainTransaction transaction);
    Task<bool> TransactionExistsAsync(string transactionHash);
    Task<IEnumerable<BlockchainTransaction>> GetTransactionsPagedAsync(int page, int pageSize);
    Task<IEnumerable<BlockchainTransaction>> GetFailedTransactionsAsync();
    Task<long> GetTransactionCountAsync();
    Task<BlockchainTransaction?> GetByTransactionHashAsync(string transactionHash);
}