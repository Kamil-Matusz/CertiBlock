using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Ethereum.Core.Exceptions;

public class EthereumTransactionsNotFoundException(Guid id) : CustomException($"Eth transaction with ID: '{id}' was not found.")
{
    public Guid Id { get; set; } = id;
}