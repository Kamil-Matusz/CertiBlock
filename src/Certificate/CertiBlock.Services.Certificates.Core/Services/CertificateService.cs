using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CertiBlock.Services.Certificates.Core.DAL.Repositories;
using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Entities;
using CertiBlock.Services.Certificates.Core.Events;
using CertiBlock.Services.Certificates.Core.Exceptions;
using CertiBlock.Services.Certificates.Core.Mappers;
using CertiBlock.Shared.DTO;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Certificates.Core.Services;

public class CertificateService(ICertificateRepository certificateRepository, IBus bus,
    ILogger<CertificateService> logger) : ICertificateService
{
    public async Task<CertificateResponse> RegisterCertificateAsync(CertificateRequest request, UserContext userContext)
    {
        var json = JsonSerializer.Serialize(request);
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        var certificateHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        
        var entity = new Certificate
        {
            OwnerName = request.OwnerName,
            Title = request.Title,
            IssuedBy = request.IssuedBy,
            IssuedDate = request.IssuedDate,
            CertificateHash = certificateHash,
            Blockchain = request.Blockchain,
            IssuerId = userContext.UserId,
        };
        
        await certificateRepository.SaveCertificateAsync(entity);
        
        try
        {
            await bus.Send(new CertificateRegistered(
                entity.Id,
                certificateHash,
                request.IssuedBy,
                request.Blockchain
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send CertificateRegistered message for CertificateId={CertificateId}", entity.Id);
        }
        
        return new CertificateResponse
        {
            CertificateId = entity.Id,
            CertificateHash = certificateHash,
            Blockchain = entity.Blockchain,
            RegisteredAt = entity.CreatedAt
        };
    }

    public async Task DeleteCertificateAsync(Guid id)
    {
        var certificate = await certificateRepository.GetCertificateByIdAsync(id);
        if (certificate is null)
        {
            throw new CertificateNotFoundException(id);
        }

        await certificateRepository.DeleteCertificateAsync(id);
    }

    public async Task<IEnumerable<CertificateDto>> GetAllCertificatesAsync()
    {
        var certificates = await certificateRepository.GetAllCertificatesAsync();
        return CertificateMapper.MapAll<CertificateDto>(certificates);
    }

    public async Task<IEnumerable<CertificateDto>> GetCertificatesByUserIdAsync(Guid userId)
    {
        var certificates = await certificateRepository.GetCertificateByIssuerIdAsync(userId.ToString());

        if (!certificates.Any())
        {
            throw new CetrtificateForUserNotFoundException(userId);
        }

        return CertificateMapper.MapAll<CertificateDto>(certificates);
    }

    public async Task<CertificateDto> GetCertificateByIdAsync(Guid certificateId)
    {
        var certificate = await certificateRepository.GetCertificateByIdAsync(certificateId);

        if (certificate is null)
        {
            throw new CertificateNotFoundException(certificateId);
        }

        return CertificateMapper.Map<CertificateDto>(certificate);
    }
}