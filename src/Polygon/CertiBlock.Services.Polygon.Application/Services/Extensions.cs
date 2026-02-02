using CertiBlock.Services.Polygon.Application.Services.CoinGecko;
using CertiBlock.Services.Polygon.Application.Services.Polygon;
using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Application.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPolygonService, PolygonService>();
        services.AddScoped<IPolygonMetricService, PolygonMetricService>();
        services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        
        return services;
    }
}