namespace CertiBlock.Services.Ethereum.Application.Services.CoinGecko;

public interface ICoinGeckoService
{
    Task<decimal> GetPriceUsdAsync(string assetId);
}