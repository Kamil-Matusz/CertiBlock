using System.Net.Http.Json;
using CertiBlock.Services.Certificates.Core.DTO;

namespace CertiBlock.Services.Certificates.Core.Clients;

public class BlockchainRouterClient : IBlockchainRouterClient
{
    private readonly HttpClient _httpClient;

    public BlockchainRouterClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /*public async Task<string> SendToBlockchainAsync(BlockchainRegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/blockchain/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ApplicationException($"BlockchainRouter error: {response.StatusCode} - {error}");
        }

        var txHash = await response.Content.ReadAsStringAsync();
        return txHash;
    }*/


    public Task<string> SendToBlockchainAsync(BlockchainRegisterRequest request)
    {
        return Task.FromResult("0xMOCKED_TRANSACTION_HASH");
    }
}