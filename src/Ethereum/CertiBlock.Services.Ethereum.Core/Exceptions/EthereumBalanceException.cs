using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Ethereum.Core.Exceptions;

public class EthereumBalanceException(string address) : CustomException($"Failed to get ETH balance for address: '{address}")
{
    public string Address { get; } = address;
}