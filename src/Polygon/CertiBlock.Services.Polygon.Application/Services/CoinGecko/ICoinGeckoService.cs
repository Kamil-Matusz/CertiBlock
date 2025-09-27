namespace CertiBlock.Services.Polygon.Application.Services.CoinGecko;

public interface ICoinGeckoService
{
    Task<decimal> GetPriceUsdAsync(string assetId);
}