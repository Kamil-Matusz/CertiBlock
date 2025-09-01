using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Ethereum.Application.Services;

public interface IEthereumService
{
    Task<BlockchainTransactionResultDto> RegisterEthereumTransactionAsync(BlockchainTransactionDto blockchainTransactionDto);
    Task<IEnumerable<BlockchainTransactionResultDto>> GetAllEthereumTransactionsAsync();
    Task<BlockchainTransactionDto> GetEthereumTransactionByIdAsync(Guid id);
    Task<BlockchainTransactionDto> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId);
    Task DeleteEthereumTransactionAsync(Guid id);
    Task<EthereumBalanceDto> GetEthBalanceAsync(string walletAddress);
    Task<BlockchainTransactionStatusDto> GetEthereumTransactionStatusAsync(string transactionHash);
    Task<BlockchainTransactionResultDto> GetTransactionByHashAsync(string txnHash);
    Task<IEnumerable<BlockchainTransactionDto>> GetEthereumTransactionsByStatusAsync(Status status);
    Task<IEnumerable<BlockchainTransactionDto>> GetEthereumTransactionsByStatusAsync(params Status[] statuses);
    Task<IEnumerable<BlockchainTransactionResultDto>> GetEthereumTransactionsPagedAsync(int page, int pageSize);
    Task<long> GetEthereumTransactionCountAsync();
}