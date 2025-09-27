using CertiBlock.Services.Ethereum.Application.Services.CoinGecko;
using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IEthereumService, EthereumService>();
        services.AddScoped<ICoinGeckoService, CoinGeckoService>();
        
        return services;
    }
}