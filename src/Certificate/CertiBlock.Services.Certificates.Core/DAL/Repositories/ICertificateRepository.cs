using CertiBlock.Services.Certificates.Core.Entities;

namespace CertiBlock.Services.Certificates.Core.DAL.Repositories;

public interface ICertificateRepository
{
    Task SaveCertificateAsync(Certificate certificate);
    Task<Certificate> GetCertificateByIdAsync(Guid id);
    Task<IEnumerable<Certificate>> GetAllCertificatesAsync();
    Task<IEnumerable<Certificate>> GetCertificateByIssuerIdAsync(string issuerId);
}