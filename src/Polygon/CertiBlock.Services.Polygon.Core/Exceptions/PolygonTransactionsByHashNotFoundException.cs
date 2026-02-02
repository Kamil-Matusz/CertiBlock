using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Polygon.Core.Exceptions;

public class PolygonTransactionsByHashNotFoundException(string transactionHash)
    : CustomException($"Eth transaction with hash: '{transactionHash}' was not found.")
{
    public string TransactionHash { get; set; } = transactionHash;
}