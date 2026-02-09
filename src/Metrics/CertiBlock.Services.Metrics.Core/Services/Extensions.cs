using CertiBlock.Services.Metrics.Core.Services.Metrics;
using CertiBlock.Services.Metrics.Core.Services.Research;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Metrics.Core.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMetricsService, MetricsService>();
        services.AddScoped<IResearchService, ResearchService>();

        services.AddHttpClient("EthereumService", client =>
        {
            client.BaseAddress = new Uri(configuration["Services:EthereumUrl"]!);
        });

        services.AddHttpClient("PolygonService", client =>
        {
            client.BaseAddress = new Uri(configuration["Services:PolygonUrl"]!);
        });

        return services;
    }
}