using CertiBlock.Services.Polygon.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IPolygonService, PolygonService>();
        return services;
    }
}