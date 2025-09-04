using CertiBlock.Services.Ethereum.Core.Ethereum;
using CertiBlock.Services.Ethereum.Core.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        // FluentValidator
        services.AddFluentValidator();
        
        // Ethereum
        services.AddEthereum(configuration);
        
        return services;
    }
}