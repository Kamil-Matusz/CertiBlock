namespace CertiBlock.Services.Certificates.Core.DTO;

public class CertificateRequest
{
    public string OwnerName { get; set; }
    public string Title { get; set; }
    public string IssuedBy { get; set; }
    public DateTime IssuedDate { get; set; }
    public string Blockchain { get; set; }
}