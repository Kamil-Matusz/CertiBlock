namespace CertiBlock.Services.Blockchain.Core.DTO;

public record CertificateRegistrationResponse(
    Guid CertificateId,
    string CertificateHash,
    string Blockchain,
    string TransactionHash,
    DateTime Timestamp);