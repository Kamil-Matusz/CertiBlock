using CertiBlock.Services.Polygon.Application.Mappers;
using CertiBlock.Services.Polygon.Core.DTO;
using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Services.Polygon.Core.Exceptions;
using CertiBlock.Services.Polygon.Core.Polygon;
using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace CertiBlock.Services.Polygon.Application.Services.Polygon;

public class PolygonService(IPolygonRepository polygonRepository, ILogger<PolygonService> logger, IWeb3 web3, 
    PolygonOptions polygonOptions) : IPolygonService
{
    public async Task<BlockchainTransactionResultDto> RegisterPolygonTransactionAsync(BlockchainTransactionDto dto)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(polygonOptions.PrivateKey);
            var web3WithAccount = new Web3(account, polygonOptions.InfuraUrl);

            var data = dto.CertificateHash.StartsWith("0x")
                ? dto.CertificateHash
                : "0x" + dto.CertificateHash;
            
            var txnInput = new TransactionInput
            {
                From = account.Address,
                To = account.Address,
                Value = new HexBigInteger(0),
                Gas = new HexBigInteger(100000),
                Data = data
            };
            
            var txnHash = await web3WithAccount.Eth.TransactionManager.SendTransactionAsync(txnInput);

            var blockchainTransaction = new BlockchainTransaction
            {
                Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                CertificateId = dto.CertificateId,
                CertificateHash = dto.CertificateHash,
                Issuer = dto.Issuer,
                Blockchain = Blockchain.Polygon,
                TransactionHash = txnHash,
                Status = Status.Submitted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await polygonRepository.SaveBlockchainTransactionAsync(blockchainTransaction);

            return BlockchainTransactionMapper.MapToResultDto(blockchainTransaction);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while registering Ethereum transaction for certificate {CertificateId}", dto.CertificateId);
            throw;
        }
    }
    
    public async Task<IEnumerable<BlockchainTransactionResultDto>> GetAllPolygonTransactionsAsync()
    {
        var blockchainTransactions = await polygonRepository.GetAllBlockchainTransactionsAsync();
        return BlockchainTransactionMapper.MapAllToResultDto(blockchainTransactions);
    }

    public async Task<BlockchainTransactionDto> GetPolygonTransactionByIdAsync(Guid id)
    {
        var ethereumBlockchain = await polygonRepository.GetBlockchainTransactionByIdAsync(id);

        if (ethereumBlockchain is null)
        {
            throw new PolygonTransactionsNotFoundException(id);
        }

        return BlockchainTransactionMapper.Map<BlockchainTransactionDto>(ethereumBlockchain);
    }

    public async Task<BlockchainTransactionDto> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId)
    {
        var ethereumBlockchain = await polygonRepository.GetBlockchainTransactionByCertificateIdAsync(certificateId);

        if (ethereumBlockchain is null)
        {
            throw new PolygonTransactionsByCertificateIdNotFoundException(certificateId);
        }

        return BlockchainTransactionMapper.Map<BlockchainTransactionDto>(ethereumBlockchain);
    }

    public async Task DeletePolygonTransactionAsync(Guid id)
    {
        var polygonTransaction = await polygonRepository.GetBlockchainTransactionByIdAsync(id);
        if (polygonTransaction is null)
        {
            throw new PolygonTransactionsNotFoundException(id);
        }

        await polygonRepository.DeleteBlockchainTransactionAsync(id);
    }

    public async Task<PolygonBalanceDto> GetPolygonBalanceAsync(string walletAddress)
    {
        try
        {
            var balanceWei = await web3.Eth.GetBalance.SendRequestAsync(walletAddress);
            var balanceEth = Web3.Convert.FromWei(balanceWei);
        
            return PolygonBalanceMapper.MapToDto(walletAddress, balanceEth);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while fetching ETH balance for {Address}: {ErrorMessage}", walletAddress, ex.Message);
            throw new PolygonBalanceException(walletAddress);
        }
    }

    public async Task<BlockchainTransactionStatusDto> GetPolygonTransactionStatusAsync(string transactionHash)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(polygonOptions.PrivateKey);
            var web3WithAccount = new Web3(account, polygonOptions.InfuraUrl);
            
            var transaction = await web3WithAccount.Eth.Transactions
                .GetTransactionByHash.SendRequestAsync(transactionHash);
            
            var receipt = await web3WithAccount.Eth.Transactions
                .GetTransactionReceipt.SendRequestAsync(transactionHash);

            if (transaction is null)
            {
                return new BlockchainTransactionStatusDto
                {
                    TransactionHash = transactionHash,
                    Status = nameof(Status.Failed),
                    BlockNumber = null,
                    Confirmations = 0,
                    InputData = null
                };
            }

            var confirmations = 0;
            Status newStatus;
            
            if (receipt == null || receipt.BlockNumber == null)
            {
                newStatus = Status.Submitted;
            }
            else
            {
                var latestBlock = await web3WithAccount.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                confirmations = (int)(latestBlock.Value - receipt.BlockNumber.Value);

                newStatus = receipt.Status.Value == 1
                    ? Status.Confirmed
                    : Status.Failed;
            }
            
            var dbTransaction = await polygonRepository.GetByTransactionHashAsync(transactionHash);
            if (dbTransaction != null)
            {
                dbTransaction.Status = newStatus;
                dbTransaction.UpdatedAt = DateTime.UtcNow;
                await polygonRepository.UpdateBlockchainTransactionAsync(dbTransaction);
            }

            return new BlockchainTransactionStatusDto
            {
                TransactionHash = transactionHash,
                Status = newStatus.ToString(),
                BlockNumber = receipt?.BlockNumber?.Value,
                Confirmations = confirmations,
                InputData = transaction.Input
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while checking Ethereum transaction {TransactionHash}", transactionHash);
            throw;
        }
    }

    public async Task<BlockchainTransactionResultDto> GetTransactionByHashAsync(string txnHash)
    {
        var transaction = await polygonRepository.GetByTransactionHashAsync(txnHash);
        
        if (transaction is null)
        {
            throw new PolygonTransactionsNotFoundException(txnHash);
        }
        
        return BlockchainTransactionMapper.MapToResultDto(transaction);
    }

    public async Task<IEnumerable<BlockchainTransactionDto>> GetPolygonTransactionsByStatusAsync(Status status)
    {
        var transactions = await polygonRepository.GetTransactionsByStatusAsync(status);
        return BlockchainTransactionMapper.MapAll<BlockchainTransactionDto>(transactions);
    }

    public async Task<IEnumerable<BlockchainTransactionDto>> GetPolygonTransactionsByStatusAsync(params Status[] statuses)
    {
        var transactions = await polygonRepository.GetTransactionsByStatusAsync(statuses);
        return BlockchainTransactionMapper.MapAll<BlockchainTransactionDto>(transactions);
    }

    public async Task<IEnumerable<BlockchainTransactionResultDto>> GetPolygonTransactionsPagedAsync(int page, int pageSize)
    {
        var transactions = await polygonRepository.GetTransactionsPagedAsync(page, pageSize);
        return BlockchainTransactionMapper.MapAllToResultDto(transactions);
    }

    public async Task<long> GetPolygonTransactionCountAsync() => await  polygonRepository.GetTransactionCountAsync();
    public async Task DeletePolygonTransactionByCertificateIdAsync(Guid certificateId)
    {
        var polygonTransaction = await polygonRepository.GetBlockchainTransactionByIdAsync(certificateId);
        if (polygonTransaction is null)
        {
            throw new PolygonTransactionsNotFoundException(certificateId);
        }

        await polygonRepository.DeleteBlockchainTransactionAsync(certificateId);
    }
}