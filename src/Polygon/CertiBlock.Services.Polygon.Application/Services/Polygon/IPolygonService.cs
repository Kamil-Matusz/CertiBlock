using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Polygon.Application.Services.Polygon;

public interface IPolygonService
{
    Task<BlockchainTransactionResultDto> RegisterPolygonTransactionAsync(BlockchainTransactionDto blockchainTransactionDto);
    Task<IEnumerable<BlockchainTransactionResultDto>> GetAllPolygonTransactionsAsync();
    Task<BlockchainTransactionDto> GetPolygonTransactionByIdAsync(Guid id);
    Task<BlockchainTransactionDto> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId);
    Task DeletePolygonTransactionAsync(Guid id);
    Task<PolygonBalanceDto> GetPolygonBalanceAsync(string walletAddress);
    Task<BlockchainTransactionStatusDto> GetPolygonTransactionStatusAsync(string transactionHash);
    Task<BlockchainTransactionResultDto> GetTransactionByHashAsync(string txnHash);
    Task<IEnumerable<BlockchainTransactionDto>> GetPolygonTransactionsByStatusAsync(Status status);
    Task<IEnumerable<BlockchainTransactionDto>> GetPolygonTransactionsByStatusAsync(params Status[] statuses);
    Task<IEnumerable<BlockchainTransactionResultDto>> GetPolygonTransactionsPagedAsync(int page, int pageSize);
    Task<long> GetPolygonTransactionCountAsync();
}