using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nethereum.Web3;

namespace CertiBlock.Services.Polygon.Core.Polygon;

public static class Extensions
{
    private const string EthereumSectionName = "Polygon";

    public static IServiceCollection AddPolygon(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(EthereumSectionName);
        
        var options = section.Get<PolygonOptions>() ?? throw new ArgumentException("Polygon configuration is missing");

        if (string.IsNullOrWhiteSpace(options.InfuraUrl))
            throw new ArgumentException("Polygon InfuraUrl is not configured properly");
        
        services.Configure<PolygonOptions>(section);
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