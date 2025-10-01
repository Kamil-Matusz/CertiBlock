using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Shared.CoinGecko;

public static class Extensions
{
    private const string CoinGeckoSectionName = "CoinGecko";

    public static IServiceCollection AddCoinGecko(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(CoinGeckoSectionName);
        services.Configure<CoinGeckoOptions>(section);

        return services;
    }
    
    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        return options;
    }
}