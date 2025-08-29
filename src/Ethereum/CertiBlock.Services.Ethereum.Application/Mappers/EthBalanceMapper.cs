using CertiBlock.Services.Ethereum.Core.DTO;

namespace CertiBlock.Services.Ethereum.Application.Mappers;

public static class EthBalanceMapper
{
    public static EthereumBalanceDto MapToDto(string address, decimal balance) => new()
    {
        Address = address,
        Balance = balance,
        Unit = "ETH"
    };

    public static EthereumBalanceDto MapToDto(string address, decimal balance, string unit) => new()
    {
        Address = address,
        Balance = balance,
        Unit = unit
    };
}