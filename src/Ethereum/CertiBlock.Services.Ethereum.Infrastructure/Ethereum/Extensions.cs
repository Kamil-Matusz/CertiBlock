using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;

namespace CertiBlock.Services.Ethereum.Infrastructure.Ethereum;

public static class Extensions
{
    private const string EthereumSectionName = "Ethereum";

    public static IServiceCollection AddEthereum(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(EthereumSectionName);
        
        var options = section.Get<EthereumOptions>() ?? throw new ArgumentException("Ethereum configuration is missing");

        if (string.IsNullOrWhiteSpace(options.InfuraUrl))
            throw new ArgumentException("Ethereum InfuraUrl is not configured properly");
        
        services.Configure<EthereumOptions>(section);
        services.AddSingleton(options);
        
        services.AddSingleton<IWeb3>(_ => new Web3(options.InfuraUrl));

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