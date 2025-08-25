using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Ethereum.Application.DTO;

public class BlockchainTransactionDto
{
    public Guid Id { get; set; }
    public Guid CertificateId { get; set; }
    public string CertificateHash { get; set; }
    public Blockchain Blockchain { get; set; }
    public string TransactionHash { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? FailedAt { get; set; }
}