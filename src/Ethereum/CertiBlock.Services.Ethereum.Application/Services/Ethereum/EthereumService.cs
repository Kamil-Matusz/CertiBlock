using CertiBlock.Services.Ethereum.Application.Mappers;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Ethereum;
using CertiBlock.Services.Ethereum.Core.Exceptions;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace CertiBlock.Services.Ethereum.Application.Services.Ethereum;

public class EthereumService(IEthereumRepository ethereumRepository, ILogger<EthereumService> logger, IWeb3 web3, 
    EthereumOptions ethereumOptions) : IEthereumService
{
    public async Task<BlockchainTransactionResultDto> RegisterEthereumTransactionAsync(BlockchainTransactionDto dto)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(ethereumOptions.PrivateKey);
            var web3WithAccount = new Web3(account, ethereumOptions.InfuraUrl);

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
                Blockchain = Blockchain.Ethereum,
                TransactionHash = txnHash,
                Status = Status.Submitted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await ethereumRepository.SaveBlockchainTransactionAsync(blockchainTransaction);

            return BlockchainTransactionMapper.MapToResultDto(blockchainTransaction);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while registering Ethereum transaction for certificate {CertificateId}", dto.CertificateId);
            throw;
        }
    }
    
    public async Task<IEnumerable<BlockchainTransactionResultDto>> GetAllEthereumTransactionsAsync()
    {
        var blockchainTransactions = await ethereumRepository.GetAllBlockchainTransactionsAsync();
        return BlockchainTransactionMapper.MapAllToResultDto(blockchainTransactions);
    }

    public async Task<BlockchainTransactionDto> GetEthereumTransactionByIdAsync(Guid id)
    {
        var ethereumBlockchain = await ethereumRepository.GetBlockchainTransactionByIdAsync(id);

        if (ethereumBlockchain is null)
        {
            throw new EthereumTransactionsNotFoundException(id);
        }

        return BlockchainTransactionMapper.Map<BlockchainTransactionDto>(ethereumBlockchain);
    }

    public async Task<BlockchainTransactionDto> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId)
    {
        var ethereumBlockchain = await ethereumRepository.GetBlockchainTransactionByCertificateIdAsync(certificateId);

        if (ethereumBlockchain is null)
        {
            throw new EthereumTransactionsByCertificateIdNotFoundException(certificateId);
        }

        return BlockchainTransactionMapper.Map<BlockchainTransactionDto>(ethereumBlockchain);
    }

    public async Task DeleteEthereumTransactionAsync(Guid id)
    {
        var ethereumTransaction = await ethereumRepository.GetBlockchainTransactionByIdAsync(id);
        if (ethereumTransaction is null)
        {
            throw new EthereumTransactionsNotFoundException(id);
        }

        await ethereumRepository.DeleteBlockchainTransactionAsync(id);
    }

    public async Task<EthereumBalanceDto> GetEthBalanceAsync(string walletAddress)
    {
        try
        {
            var balanceWei = await web3.Eth.GetBalance.SendRequestAsync(walletAddress);
            var balanceEth = Web3.Convert.FromWei(balanceWei);
        
            return EthBalanceMapper.MapToDto(walletAddress, balanceEth);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while fetching ETH balance for {Address}: {ErrorMessage}", walletAddress, ex.Message);
            throw new EthereumBalanceException(walletAddress);
        }
    }

    public async Task<BlockchainTransactionStatusDto> GetEthereumTransactionStatusAsync(string transactionHash)
    {
        try
        {
            var account = new Nethereum.Web3.Accounts.Account(ethereumOptions.PrivateKey);
            var web3WithAccount = new Web3(account, ethereumOptions.InfuraUrl);
            
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
            
            var dbTransaction = await ethereumRepository.GetByTransactionHashAsync(transactionHash);
            if (dbTransaction != null)
            {
                dbTransaction.Status = newStatus;
                dbTransaction.UpdatedAt = DateTime.UtcNow;
                await ethereumRepository.UpdateBlockchainTransactionAsync(dbTransaction);
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
        var transaction = await ethereumRepository.GetByTransactionHashAsync(txnHash);
        
        if (transaction is null)
        {
            throw new EthereumTransactionsNotFoundException(txnHash);
        }
        
        return BlockchainTransactionMapper.MapToResultDto(transaction);
    }

    public async Task<IEnumerable<BlockchainTransactionDto>> GetEthereumTransactionsByStatusAsync(Status status)
    {
        var transactions = await ethereumRepository.GetTransactionsByStatusAsync(status);
        return BlockchainTransactionMapper.MapAll<BlockchainTransactionDto>(transactions);
    }

    public async Task<IEnumerable<BlockchainTransactionDto>> GetEthereumTransactionsByStatusAsync(params Status[] statuses)
    {
        var transactions = await ethereumRepository.GetTransactionsByStatusAsync(statuses);
        return BlockchainTransactionMapper.MapAll<BlockchainTransactionDto>(transactions);
    }

    public async Task<IEnumerable<BlockchainTransactionResultDto>> GetEthereumTransactionsPagedAsync(int page, int pageSize)
    {
        var transactions = await ethereumRepository.GetTransactionsPagedAsync(page, pageSize);
        return BlockchainTransactionMapper.MapAllToResultDto(transactions);
    }

    public async Task<long> GetEthereumTransactionCountAsync() => await ethereumRepository.GetTransactionCountAsync();
    public async Task DeleteEthereumTransactionByCertificateIdAsync(Guid certificateId)
    {
        var ethereumTransaction = await ethereumRepository.GetBlockchainTransactionByCertificateIdAsync(certificateId);
        if (ethereumTransaction is null)
        {
            throw new EthereumTransactionsNotFoundException(certificateId);
        }

        await ethereumRepository.DeleteBlockchainTransactionByCertificateIdAsync(certificateId);
    }
}