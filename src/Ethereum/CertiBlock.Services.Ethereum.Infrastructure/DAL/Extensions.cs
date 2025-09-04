using CertiBlock.Services.Ethereum.Core.Repositories;
using CertiBlock.Services.Ethereum.Infrastructure.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Infrastructure.DAL;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IEthereumRepository, EthereumRepository>();
        
        return services;
    }
}