using System.Text.Json;
using CertiBlock.Shared.CoinGecko;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CertiBlock.Services.Ethereum.Application.Services.CoinGecko;

public class CoinGeckoService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<CoinGeckoService> logger, IOptions<CoinGeckoOptions> options) 
    : ICoinGeckoService, IDisposable
{
    private readonly CoinGeckoOptions _options = options.Value;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(options.Value.CacheExpiryMinutes);
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private const string CacheKeyPrefix = "CoinGecko_Price_";
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };

    public async Task<decimal> GetPriceUsdAsync(string assetId)
    {
        if (string.IsNullOrWhiteSpace(assetId))
        {
            logger.LogWarning("Empty assetId provided, returning fallback price");
            return _options.FallbackPrice;
        }

        var cacheKey = $"{CacheKeyPrefix}{assetId}";
        if (memoryCache.TryGetValue(cacheKey, out decimal cachedPrice))
        {
            logger.LogDebug("Cache hit for {AssetId}, returning cached price: ${Price}", assetId, cachedPrice);
            return cachedPrice;
        }
        
        await _semaphore.WaitAsync();
        try
        {
            if (memoryCache.TryGetValue(cacheKey, out cachedPrice))
            {
                logger.LogDebug("Cache hit after lock for {AssetId}", assetId);
                return cachedPrice;
            }

            return await FetchAndCachePriceAsync(assetId, cacheKey);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<decimal> FetchAndCachePriceAsync(string assetId, string cacheKey)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(_options.ApiKey)
                ? $"{_options.ApiUrl}?ids={assetId}&vs_currencies=usd"
                : $"{_options.ApiUrl}?ids={assetId}&vs_currencies=usd&x_cg_demo_api_key={_options.ApiKey}";
            
            using var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("CoinGecko API returned {StatusCode} for {AssetId}. Using fallback price", response.StatusCode, assetId);
                CacheFallbackPrice(cacheKey);
                return _options.FallbackPrice;
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(
                content, JsonOptions);

            if (data?.TryGetValue(assetId, out var assetData) == true && 
                assetData.TryGetValue("usd", out var price) && 
                price > 0)
            {
                CachePrice(cacheKey, price);
                logger.LogInformation("Fetched and cached price for {AssetId}: ${Price}", assetId, price);
                return price;
            }

            logger.LogWarning("Asset {AssetId} not found or has invalid price in CoinGecko response. Using fallback", assetId);
            CacheFallbackPrice(cacheKey);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error fetching {AssetId} price from CoinGecko", assetId);
            CacheFallbackPrice(cacheKey);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Request timeout fetching {AssetId} price from CoinGecko", assetId);
            CacheFallbackPrice(cacheKey);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON deserialization error for {AssetId}", assetId);
            CacheFallbackPrice(cacheKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error fetching {AssetId} price", assetId);
            CacheFallbackPrice(cacheKey);
        }

        return _options.FallbackPrice;
    }

    private void CachePrice(string cacheKey, decimal price)
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_cacheExpiry)
            .SetSize(1);

        memoryCache.Set(cacheKey, price, cacheOptions);
    }

    private void CacheFallbackPrice(string cacheKey)
    {
        var shortCacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
            .SetSize(1);

        memoryCache.Set(cacheKey, _options.FallbackPrice, shortCacheOptions);
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}