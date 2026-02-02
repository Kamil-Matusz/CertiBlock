using System.Collections.Concurrent;
using System.Text.Json;
using CertiBlock.Shared.CoinGecko;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CertiBlock.Services.Polygon.Application.Services.CoinGecko;

public class CoinGeckoService(HttpClient httpClient, ILogger<CoinGeckoService> logger, IOptions<CoinGeckoOptions> options) 
    : ICoinGeckoService, IDisposable
{
    private readonly CoinGeckoOptions _options = options.Value;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(options.Value.CacheExpiryMinutes);
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };

    private record CacheEntry(decimal Price, DateTime Timestamp);

    public async Task<decimal> GetPriceUsdAsync(string assetId)
    {
        if (string.IsNullOrWhiteSpace(assetId))
        {
            logger.LogWarning("Empty assetId provided, returning fallback price");
            return _options.FallbackPrice;
        }
        
        if (_cache.TryGetValue(assetId, out var cached) && 
            DateTime.UtcNow - cached.Timestamp < _cacheExpiry)
        {
            logger.LogDebug("Cache hit for {AssetId}, returning cached price: ${Price}", assetId, cached.Price);
            return cached.Price;
        }
        
        await _semaphore.WaitAsync();
        try
        {
            if (_cache.TryGetValue(assetId, out cached) && 
                DateTime.UtcNow - cached.Timestamp < _cacheExpiry)
            {
                logger.LogDebug("Cache hit after lock for {AssetId}", assetId);
                return cached.Price;
            }

            return await FetchAndCachePriceAsync(assetId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<decimal> FetchAndCachePriceAsync(string assetId)
    {
        try
        {
            var url = $"{_options.ApiUrl}?ids={assetId}&vs_currencies=usd";
            
            using var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "CoinGecko API returned {StatusCode} for {AssetId}. Using fallback price", 
                    response.StatusCode, assetId);
                CacheFallbackPrice(assetId);
                return _options.FallbackPrice;
            }

            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(
                content, JsonOptions);

            if (data?.TryGetValue(assetId, out var assetData) == true && 
                assetData.TryGetValue("usd", out var price) && 
                price > 0)
            {
                _cache[assetId] = new CacheEntry(price, DateTime.UtcNow);
                logger.LogInformation("Fetched and cached price for {AssetId}: ${Price}", assetId, price);
                
                CleanupExpiredEntries();
                
                return price;
            }

            logger.LogWarning(
                "Asset {AssetId} not found or has invalid price in CoinGecko response. Using fallback", 
                assetId);
            CacheFallbackPrice(assetId);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP error fetching {AssetId} price from CoinGecko", assetId);
            CacheFallbackPrice(assetId);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogError(ex, "Request timeout fetching {AssetId} price from CoinGecko", assetId);
            CacheFallbackPrice(assetId);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "JSON deserialization error for {AssetId}", assetId);
            CacheFallbackPrice(assetId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error fetching {AssetId} price", assetId);
            CacheFallbackPrice(assetId);
        }

        return _options.FallbackPrice;
    }

    private void CacheFallbackPrice(string assetId)
    {
        _cache[assetId] = new CacheEntry(_options.FallbackPrice, DateTime.UtcNow.AddMinutes(-58));
    }

    private void CleanupExpiredEntries()
    {
        if (_cache.Count <= 100) return;

        var now = DateTime.UtcNow;
        var expiredKeys = _cache
            .Where(kvp => now - kvp.Value.Timestamp > _cacheExpiry)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _cache.TryRemove(key, out _);
        }

        if (expiredKeys.Count > 0)
        {
            logger.LogDebug("Cleaned up {Count} expired cache entries", expiredKeys.Count);
        }
    }

    public void Dispose()
    {
        _semaphore?.Dispose();
    }
}