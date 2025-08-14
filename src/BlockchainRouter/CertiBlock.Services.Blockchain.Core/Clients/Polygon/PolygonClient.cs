using CertiBlock.Services.Blockchain.Core.Events;

namespace CertiBlock.Services.Blockchain.Core.Clients.Polygon;

public class PolygonClient(HttpClient httpClient) : IPolygonClient
{
    private readonly HttpClient _httpClient = httpClient;

    public Task RegisterCertificateAsync(CertificateRegistered certificate)
    {
        throw new NotImplementedException();
    }
}