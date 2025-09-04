using CertiBlock.Services.Certificates.Core.Clients.Ethereum;
using CertiBlock.Services.Certificates.Core.Clients.Polygon;
using CertiBlock.Services.Certificates.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CertiBlock.Services.Certificates.Core.Clients;

public static class Extensions
{
    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        var options = services.GetOptions<BlockchainClientsOptions>("BlockchainClients");
        services.AddSingleton(options);

        if (string.IsNullOrWhiteSpace(options.Ethereum))
            throw new BlockchainConfigurationException("Ethereum", "BlockchainClients:Ethereum");

        if (string.IsNullOrWhiteSpace(options.Polygon))
            throw new BlockchainConfigurationException("Polygon", "BlockchainClients:Polygon");
        
        // Polly Config
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: options.RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // Http Clients
        services.AddHttpClient<IEthereumClient, EthereumClient>(client =>
        {
            client.BaseAddress = new Uri(options.Ethereum);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        })
            .AddPolicyHandler(retryPolicy);

        services.AddHttpClient<IPolygonClient, PolygonClient>(client =>
        {
            client.BaseAddress = new Uri(options.Polygon);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        })
            .AddPolicyHandler(retryPolicy);

        return services;
    }
    
    private static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}
