using CertiBlock.Services.Ethereum.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IEthereumService, EthereumService>();
        
        return services;
    }
}