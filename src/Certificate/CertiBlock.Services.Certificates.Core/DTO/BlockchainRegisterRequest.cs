namespace CertiBlock.Services.Certificates.Core.DTO;

public class BlockchainRegisterRequest
{
    public string Hash { get; set; }
    public string Issuer { get; set; }
    public string Blockchain { get; set; }
}