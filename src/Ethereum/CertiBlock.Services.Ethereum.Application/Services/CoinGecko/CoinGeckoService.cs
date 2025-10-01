using System.Text.Json;
using CertiBlock.Shared.CoinGecko;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CertiBlock.Services.Ethereum.Application.Services.CoinGecko;

public class CoinGeckoService(HttpClient httpClient, ILogger<CoinGeckoService> logger, IOptions<CoinGeckoOptions> options) : ICoinGeckoService
{
    private readonly CoinGeckoOptions _options = options.Value;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(options.Value.CacheExpiryMinutes);
    private readonly Dictionary<string, (decimal price, DateTime timestamp)> _cache = new();

    public async Task<decimal> GetPriceUsdAsync(string assetId)
    {
        if (_cache.TryGetValue(assetId, out var cached) && DateTime.UtcNow - cached.timestamp < _cacheExpiry)
        {
            return cached.price;
        }

        try
        {
            var baseUrl = GetBaseApiUrl();
            var url = $"{baseUrl}?ids={assetId}&vs_currencies=usd";
            
            var response = await httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(response);

            if (data?.TryGetValue(assetId, out var assetData) == true && 
                assetData.TryGetValue("usd", out var price))
            {
                _cache[assetId] = (price, DateTime.UtcNow);
                return price;
            }

            logger.LogWarning("Asset {AssetId} not found in CoinGecko response. Using fallback price: ${FallbackPrice}", 
                assetId, _options.FallbackPrice);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error fetching {AssetId} price from CoinGecko", assetId);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON deserialization error for {AssetId}", assetId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error fetching {AssetId} price. Using fallback price: ${FallbackPrice}", 
                assetId, _options.FallbackPrice);
        }

        return _options.FallbackPrice;
    }

    private string GetBaseApiUrl()
    {
        var uri = new Uri(_options.ApiUrl);
        return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
    }
}