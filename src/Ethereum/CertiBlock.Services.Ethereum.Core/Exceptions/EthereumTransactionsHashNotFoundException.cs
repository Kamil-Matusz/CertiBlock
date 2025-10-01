using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Ethereum.Core.Exceptions;

public class EthereumTransactionsByHashNotFoundException(string transactionHash)
    : CustomException($"Eth transaction with hash: '{transactionHash}' was not found.")
{
    public string TransactionHash { get; set; } = transactionHash;
}