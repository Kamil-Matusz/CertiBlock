namespace CertiBlock.Services.Certificates.Core.DTO;

public record CertificateRegistrationRequest(
    Guid CertificateId, 
    string CertificateHash, 
    string Issuer);