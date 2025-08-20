namespace CertiBlock.Services.Blockchain.Core.DTO;

public record CertificateRegistrationRequest(
    Guid CertificateId, 
    string CertificateHash, 
    string Issuer);