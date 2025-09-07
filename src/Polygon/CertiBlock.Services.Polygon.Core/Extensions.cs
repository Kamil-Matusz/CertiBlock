using CertiBlock.Services.Polygon.Core.Polygon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        // Ethereum
        services.AddPolygon(configuration);
        
        return services;
    }
}