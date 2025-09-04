namespace CertiBlock.Services.Certificates.Core.DTO;

public record CertificateRegistrationResponse(
    Guid CertificateId,
    string CertificateHash,
    string Blockchain,
    string TransactionHash,
    DateTime Timestamp);