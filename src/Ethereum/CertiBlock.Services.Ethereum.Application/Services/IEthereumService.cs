using CertiBlock.Services.Ethereum.Core.DTO;

namespace CertiBlock.Services.Ethereum.Application.Services;

public interface IEthereumService
{
    Task<BlockchainTransactionResultDto> RegisterEthereumTransactionAsync(BlockchainTransactionDto blockchainTransactionDto);
    Task<IEnumerable<BlockchainTransactionDto>> GetAllEthereumTransactionsAsync();
    Task<BlockchainTransactionDto> GetBlockchainTransactionByIdAsync(Guid id);
    Task<BlockchainTransactionDto> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId);
    Task DeleteEthereumTransactionAsync(Guid id);
    Task<EthereumBalanceDto> GetEthBalanceAsync(string walletAddress);
    Task<BlockchainTransactionStatusDto> GetEthereumTransactionStatusAsync(string transactionHash);
}