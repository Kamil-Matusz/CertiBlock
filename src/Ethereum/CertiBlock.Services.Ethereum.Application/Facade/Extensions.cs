using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application.Facade;

public static class Extensions
{
    public static IServiceCollection AddFacade(this IServiceCollection services)
    {
        // Eth Facade
        services.AddScoped<IEthereumFacade, EthereumFacade>();
        
        return services;
    }
}