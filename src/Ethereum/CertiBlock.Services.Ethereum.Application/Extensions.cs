using CertiBlock.Services.Ethereum.Application.Facade;
using CertiBlock.Services.Ethereum.Application.Hangfire;
using CertiBlock.Services.Ethereum.Application.RabbitMQ;
using CertiBlock.Services.Ethereum.Application.Services;
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
        services.AddServices();
        
        // Hangfire
        services.AddHangfireJobs();
        
        
        // Facade
        services.AddFacade();
        
        services.AddScoped<MetricPublisher>();
        
        return services;
    }
}