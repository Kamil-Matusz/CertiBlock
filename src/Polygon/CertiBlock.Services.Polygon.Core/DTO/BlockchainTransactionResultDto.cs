using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Polygon.Core.DTO;

public class BlockchainTransactionResultDto
{
    public Guid Id { get; set; }
    public Guid CertificateId { get; set; }
    public string TransactionHash { get; set; }
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
}