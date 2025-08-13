using CertiBlock.Shared.Enums;

namespace CertiBlock.Services.Certificates.Core.DTO;

public class BlockchainRegisterRequest
{
    public Guid CertificateId { get; set; }
    public string Hash { get; set; }
    public string Issuer { get; set; }
    public Blockchain Blockchain { get; set; }
}