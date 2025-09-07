using System.Net.Http.Json;
using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Events;
using CertiBlock.Services.Certificates.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Certificates.Core.Clients.Ethereum;

public class EthereumClient(HttpClient httpClient, ILogger<EthereumClient> logger) : IEthereumClient
{
    public async Task RegisterCertificateAsync(CertificateRegistered certificate)
    {
        var request = new CertificateRegistrationRequest(
            certificate.CertificateId,
            certificate.CertificateHash,
            certificate.Issuer
        );
        
        var response = await httpClient.PostAsJsonAsync("/ethereum-service/Ethereum/registerCertificate", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BlockchainTransactionFailedException(certificate.CertificateId, "Ethereum", $"HTTP {response.StatusCode}: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<CertificateRegistrationResponse>();
        
        logger.LogInformation($"[Ethereum] Zarejestrowano certyfikat {result?.CertificateId}");
    }
}