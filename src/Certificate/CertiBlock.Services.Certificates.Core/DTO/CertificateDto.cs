namespace CertiBlock.Services.Certificates.Core.DTO;

public class CertificateDto
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public string Title { get; set; }
    public string IssuedBy { get; set; }
    public DateTime IssuedDate { get; set; }
    public string CertificateHash { get; set; }
    public string TransactionHash { get; set; }
    public string Blockchain { get; set; }
    public string IssuerId { get; set; }
    public DateTime CreatedAt { get; set; }
}