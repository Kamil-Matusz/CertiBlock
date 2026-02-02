using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Certificates.Core.Exceptions;

public class BlockchainTransactionFailedException(Guid certificateId, string blockchain, string reason)
    : CustomException($"Certificate {certificateId} registration on blockchain {blockchain} failed. Reason: {reason}")
{
    public Guid CertificateId { get; } = certificateId;
    public string Blockchain { get; } = blockchain;
}