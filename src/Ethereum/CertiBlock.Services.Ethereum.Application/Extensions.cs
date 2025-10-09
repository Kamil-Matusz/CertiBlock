using CertiBlock.Services.Ethereum.Application.RabbitMQ;
using CertiBlock.Services.Ethereum.Application.Services.CoinGecko;
using CertiBlock.Services.Ethereum.Application.Services.Ethereum;
using CertiBlock.Services.Ethereum.Application.Services.EthereumMetrics;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IEthereumService, EthereumService>();
        services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        services.AddScoped<IEthereumMetricService, EthereumMetricService>();
        services.AddScoped<MetricPublisher>();
        
        return services;
    }
}