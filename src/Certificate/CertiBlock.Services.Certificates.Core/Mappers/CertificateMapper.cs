using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Entities;

namespace CertiBlock.Services.Certificates.Core.Mappers;

public static class CertificateMapper
{
    public static T Map<T>(Certificate certificate) where T : CertificateDto, new() => new T()
    {
        Id = certificate.Id,
        OwnerName = certificate.OwnerName,
        Title = certificate.Title,
        IssuedBy = certificate.IssuedBy,
        IssuedDate = certificate.IssuedDate,
        CertificateHash = certificate.CertificateHash,
        Blockchain = certificate.Blockchain,
        IssuerId = certificate.IssuerId,
        CreatedAt = certificate.CreatedAt
    };

    public static IEnumerable<T> MapAll<T>(IEnumerable<Certificate> certificates) where T : CertificateDto, new()
    {
        return certificates.Select(certificate => Map<T>(certificate));
    }

    public static IEnumerable<CertificateDto> MapAllToDto(IEnumerable<Certificate> certificates) => 
        MapAll<CertificateDto>(certificates);
}