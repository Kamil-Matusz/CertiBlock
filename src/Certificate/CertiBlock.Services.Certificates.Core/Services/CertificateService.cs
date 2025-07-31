using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CertiBlock.Services.Certificates.Core.Clients;
using CertiBlock.Services.Certificates.Core.DAL.Repositories;
using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Entities;

namespace CertiBlock.Services.Certificates.Core.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly IBlockchainRouterClient _blockchainRouterClient;

    public CertificateService(ICertificateRepository certificateRepository, IBlockchainRouterClient blockchainRouterClient)
    {
        _certificateRepository = certificateRepository;
        _blockchainRouterClient = blockchainRouterClient;
    }

    public async Task<CertificateResponse> RegisterCertificateAsync(CertificateRequest request, UserContext userContext)
    {
        // 1. Serializuj certyfikat do JSON
        var json = JsonSerializer.Serialize(request);

        // 2. Oblicz hash SHA256
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        var certificateHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        
        var blockchainRequest = new BlockchainRegisterRequest
        {
            Hash = certificateHash,
            Issuer = request.IssuedBy,
            Blockchain = request.Blockchain
        };
        
        var txHash = await _blockchainRouterClient.SendToBlockchainAsync(blockchainRequest);

        // 4. Utwórz encję Mongo
        var entity = new Certificate
        {
            OwnerName = request.OwnerName,
            Title = request.Title,
            IssuedBy = request.IssuedBy,
            IssuedDate = request.IssuedDate,
            CertificateHash = certificateHash,
            Blockchain = request.Blockchain,
            TransactionHash = txHash,
            IssuerId = userContext.UserId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _certificateRepository.SaveCertificateAsync(entity);

        // 6. Zwróć odpowiedź
        return new CertificateResponse
        {
            TransactionHash = txHash,
            Blockchain = request.Blockchain,
            CertificateHash = certificateHash,
            RegisteredAt = entity.CreatedAt
        };
    }
}