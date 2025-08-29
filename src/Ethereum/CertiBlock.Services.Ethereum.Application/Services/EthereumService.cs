using CertiBlock.Services.Ethereum.Application.Mappers;
using CertiBlock.Services.Ethereum.Core.DTO;
using CertiBlock.Services.Ethereum.Core.Entities;
using CertiBlock.Services.Ethereum.Core.Exceptions;
using CertiBlock.Services.Ethereum.Core.Repositories;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Serilog;

namespace CertiBlock.Services.Ethereum.Application.Services;

public class EthereumService(IEthereumRepository ethereumRepository, ILogger<EthereumService> logger, IWeb3 web3) : IEthereumService
{
    public Task<BlockchainTransaction> RegisterEthereumTransactionAsync(BlockchainTransactionDto blockchainTransactionDto)
    {
        throw new NotImplementedException();
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