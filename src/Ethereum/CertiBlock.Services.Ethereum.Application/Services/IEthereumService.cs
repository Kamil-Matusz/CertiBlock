using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;

namespace CertiBlock.Services.Ethereum.Application.Services;

public interface IEthereumService
{
    Task<BlockchainTransaction> RegisterEthereumTransactionAsync(BlockchainTransactionDto blockchainTransactionDto);
    Task<IEnumerable<BlockchainTransactionDto>> GetAllEthereumTransactionsAsync();
    Task<BlockchainTransactionDto> GetBlockchainTransactionByIdAsync(Guid id);
    Task<BlockchainTransactionDto> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId);
    Task DeleteEthereumTransactionAsync(Guid id);
    Task<EthereumBalanceDto> GetEthBalanceAsync(string walletAddress);
}