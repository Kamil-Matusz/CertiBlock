using CertiBlock.Services.Polygon.Application.Services;
using CertiBlock.Services.Polygon.Application.Services.CoinGecko;
using CertiBlock.Services.Polygon.Application.Services.Polygon;
using CertiBlock.Services.Polygon.Application.Services.PolygonMetrics;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IPolygonService, PolygonService>();
        services.AddHttpClient<ICoinGeckoService, CoinGeckoService>();
        services.AddScoped<IPolygonMetricService, PolygonMetricService>();
        return services;
    }
}