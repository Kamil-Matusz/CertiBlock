using CertiBlock.Services.Blockchain.Core.Events;

namespace CertiBlock.Services.Blockchain.Core.Clients.Ethereum;

public class EthereumClient(HttpClient httpClient) : IEthereumClient
{
    private readonly HttpClient _httpClient = httpClient;

    public Task RegisterCertificateAsync(CertificateRegistered certificate)
    {
        throw new NotImplementedException();
    }
}