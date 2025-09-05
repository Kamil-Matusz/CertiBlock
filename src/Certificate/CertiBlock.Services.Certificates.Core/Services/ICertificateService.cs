using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Shared.DTO;

namespace CertiBlock.Services.Certificates.Core.Services;

public interface ICertificateService
{
    Task<CertificateResponse> RegisterCertificateAsync(CertificateRequest request, UserContext userContext);
    Task DeleteCertificateAsync(Guid id);
    Task<IEnumerable<CertificateDto>> GetAllCertificatesAsync();
    Task<IEnumerable<CertificateDto>> GetCertificatesByUserIdAsync(Guid userId);
    Task<CertificateDto> GetCertificateByIdAsync(Guid certificateId);
    Task<IEnumerable<CertificateDto>> GetCertificatesPagedAsync(int page, int pageSize);
}