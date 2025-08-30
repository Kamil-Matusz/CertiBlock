using System.Text;
using CertiBlock.Services.Ethereum.Application.Mappers;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Ethereum;
using CertiBlock.Services.Ethereum.Core.Exceptions;
using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Shared.Enums;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Serilog;

namespace CertiBlock.Services.Ethereum.Application.Services;

public class EthereumService(IEthereumRepository ethereumRepository, ILogger<EthereumService> logger, IWeb3 web3, EthereumOptions ethereumOptions) : IEthereumService
{
    public async Task<BlockchainTransaction> RegisterEthereumTransactionAsync(BlockchainTransactionDto dto)
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
                Gas = new HexBigInteger(100000)
            };
            
            var txnHash = await web3WithAccount.Eth.TransactionManager.SendTransactionAsync(txnInput);

            var blockchainTransaction = new BlockchainTransaction
            {
                Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                CertificateId = dto.CertificateId,
                CertificateHash = dto.CertificateHash,
                Blockchain = dto.Blockchain,
                TransactionHash = txnHash,
                Status = Status.Submitted,
                CreatedAt = DateTime.UtcNow
            };

            await ethereumRepository.SaveBlockchainTransactionAsync(blockchainTransaction);

            return blockchainTransaction;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while registering Ethereum transaction for certificate {CertificateId}", dto.CertificateId);
            throw;
        }
    }
    
    public async Task<IEnumerable<BlockchainTransactionDto>> GetAllEthereumTransactionsAsync()
    {
        var blockchainTransactions = await ethereumRepository.GetAllBlockchainTransactionsAsync();
        return BlockchainTransactionMapper.MapAll<BlockchainTransactionDto>(blockchainTransactions);
    }

    public async Task<BlockchainTransactionDto> GetBlockchainTransactionByIdAsync(Guid id)
    {
        var ethereumBlockchain = await ethereumRepository.GetBlockchainTransactionByIdAsync(id);

        if (ethereumBlockchain is null)
        {
            throw new EthereumTransactionsNotFoundException(id);
        }

        return BlockchainTransactionMapper.Map<BlockchainTransactionDto>(ethereumBlockchain);
    }

    public Task<BlockchainTransactionDto> GetBlockchainTransactionByCertificateIdAsync(Guid certificateId)
    {
        throw new NotImplementedException();
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
}