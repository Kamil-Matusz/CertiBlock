namespace CertiBlock.Services.Certificates.Core.Events;

public record CertificateRegistered (Guid CertificateId, string CertificateHash, string Issuer, string Blockchain);
