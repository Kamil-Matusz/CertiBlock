using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Certificates.Core.Events;

public record CertificateRegistered (Guid CertificateId, string CertificateHash, string Issuer, Blockchain Blockchain);
