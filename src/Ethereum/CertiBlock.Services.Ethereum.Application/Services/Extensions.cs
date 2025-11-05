using CertiBlock.Services.Ethereum.Application.Services.CoinGecko;
using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IEthereumService, EthereumService>();
        services.AddScoped<IEthereumMetricService, EthereumMetricService>();
        services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        
        return services;
    }
}