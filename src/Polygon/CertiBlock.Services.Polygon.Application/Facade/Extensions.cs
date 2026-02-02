using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Application.Facade;

public static class Extensions
{
    public static IServiceCollection AddFacade(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IPolygonFacade, PolygonFacade>();
        
        return services;
    }
}