using System.Net.Http.Json;
using CertiBlock.Services.Certificates.Core.DTO;
using CertiBlock.Services.Certificates.Core.Events;
using CertiBlock.Services.Certificates.Core.Exceptions;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Certificates.Core.Clients.Polygon;

public class PolygonClient(HttpClient httpClient, ILogger<PolygonClient> logger) : IPolygonClient
{
    public async Task RegisterCertificateAsync(CertificateRegistered certificate)
    {
        var request = new CertificateRegistrationRequest(
            certificate.CertificateId,
            certificate.CertificateHash,
            certificate.Issuer
        );
        
        var response = await httpClient.PostAsJsonAsync("/certificates/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BlockchainTransactionFailedException(certificate.CertificateId, "Polygon", $"HTTP {response.StatusCode}: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<CertificateRegistrationResponse>();
        
        logger.LogInformation($"[Polygon] Register certificate with ID: {result?.CertificateId}");
    }
}