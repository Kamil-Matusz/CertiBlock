using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Metrics.Core.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMetricsService, MetricsService>();
        
        return services;
    }
}