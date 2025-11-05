using CertiBlock.Services.Polygon.Application.Facade;
using CertiBlock.Services.Polygon.Application.RabbitMQ;
using CertiBlock.Services.Polygon.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddServices();
        
        // Facade
        services.AddFacade();
        
        services.AddScoped<MetricPublisher>();
        
        return services;
    }
}