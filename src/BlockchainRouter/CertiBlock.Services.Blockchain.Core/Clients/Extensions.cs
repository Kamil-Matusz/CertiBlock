using CertiBlock.Services.Blockchain.Core.Clients.Ethereum;
using CertiBlock.Services.Blockchain.Core.Clients.Polygon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Blockchain.Core.Clients;

public static class Extensions
{
    public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        var ethereumUrl = configuration["BlockchainClients:Ethereum"];
        var polygonUrl = configuration["BlockchainClients:Polygon"];

        if (string.IsNullOrWhiteSpace(ethereumUrl))
            throw new InvalidOperationException("Missing configuration for BlockchainClients:Ethereum");

        if (string.IsNullOrWhiteSpace(polygonUrl))
            throw new InvalidOperationException("Missing configuration for BlockchainClients:Polygon");

        services.AddHttpClient<IEthereumClient, EthereumClient>(client =>
        {
            client.BaseAddress = new Uri(ethereumUrl);
        });

        services.AddHttpClient<IPolygonClient, PolygonClient>(client =>
        {
            client.BaseAddress = new Uri(polygonUrl);
        });

        return services;
    }
}
