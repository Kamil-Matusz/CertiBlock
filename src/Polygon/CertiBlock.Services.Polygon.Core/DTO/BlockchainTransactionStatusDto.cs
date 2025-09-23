using System.Numerics;

namespace CertiBlock.Services.Polygon.Core.DTO;

public class BlockchainTransactionStatusDto
{
    public string TransactionHash { get; set; }
    public string Status { get; set; }
    public BigInteger? BlockNumber { get; set; }
    public int Confirmations { get; set; }
    public string InputData { get; set; }
}
