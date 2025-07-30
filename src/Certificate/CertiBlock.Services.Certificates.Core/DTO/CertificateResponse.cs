namespace CertiBlock.Services.Certificates.Core.DTO;

public class CertificateResponse
{
    public string TransactionHash { get; set; }
    public string Blockchain { get; set; }
    public string CertificateHash { get; set; }
    public DateTime RegisteredAt { get; set; }
}