using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Ethereum.Core.DTO;

public class BlockchainTransactionDto
{
    public Guid Id { get; set; }
    public Guid CertificateId { get; set; }
    public string CertificateHash { get; set; }
    public Blockchain Blockchain { get; set; }
    //public string TransactionHash { get; set; }
    //public Status Status { get; set; }
    //public DateTime CreatedAt { get; set; }
}