namespace CertiBlock.Services.Ethereum.Core.DTO;

public class EthereumBalanceDto
{
    public string Address { get; set; }
    public decimal Balance { get; set; }
    public string Unit { get; set; } = "ETH";
}