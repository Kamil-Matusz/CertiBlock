using CertiBlock.Services.Certificates.Core.DTO;

namespace CertiBlock.Services.Certificates.Core.Services;

public interface ICertificateService
{
    Task<CertificateResponse> RegisterCertificateAsync(CertificateRequest request, UserContext userContext);
}