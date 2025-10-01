namespace CertiBlock.Shared.CoinGecko;

public class CoinGeckoOptions
{
    public string ApiUrl { get; set; }
    public decimal FallbackPrice { get; set; }
    public int CacheExpiryMinutes { get; set; }
}