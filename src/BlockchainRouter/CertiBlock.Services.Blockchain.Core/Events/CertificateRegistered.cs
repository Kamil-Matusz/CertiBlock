namespace CertiBlock.Services.Blockchain.Core.Events;

public record CertificateRegistered (Guid CertificateId, string CertificateHash, string Issuer, Shared.Enums.Blockchain Blockchain);
