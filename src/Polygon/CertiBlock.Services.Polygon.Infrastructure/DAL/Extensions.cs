using CertiBlock.Services.Polygon.Core.Repositories;
using CertiBlock.Services.Polygon.Infrastructure.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Infrastructure.DAL;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPolygonRepository, PolygonRepository>();
        services.AddScoped<IPolygonMetricRepository, PolygonMetricRepository>();
        return services;
    }
}