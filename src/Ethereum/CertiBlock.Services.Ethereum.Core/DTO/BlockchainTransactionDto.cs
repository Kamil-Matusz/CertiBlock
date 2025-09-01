namespace CertiBlock.Services.Ethereum.Core.DTO;

public class BlockchainTransactionDto
{
    public Guid Id { get; set; }
    public Guid CertificateId { get; set; }
    public string CertificateHash { get; set; }
    public string Issuer { get; set; }
}