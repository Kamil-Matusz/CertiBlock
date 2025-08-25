using CertiBlock.Services.Ethereum.Core.Entities;

namespace CertiBlock.Services.Ethereum.Core.Repositories;

public interface IEthereumRepository
{
    Task<BlockchainTransaction> GetBlockchainTransactionByIdAsync(Guid id);
    Task<IEnumerable<BlockchainTransaction>> GetAllBlockchainTransactionsAsync();
    Task SaveBlockchainTransactionAsync(BlockchainTransaction blockchainTransaction);
}