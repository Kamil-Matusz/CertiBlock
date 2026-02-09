using CertiBlock.Services.Metrics.Core.RabbitMQ;
using CertiBlock.Services.Metrics.Core.Services;
using CertiBlock.Shared.CORS;
using CertiBlock.Shared.InfluxDB;
using CertiBlock.Shared.Logging;
using CertiBlock.Shared.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Metrics.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        // CORS
        services.AddCorsPolicy();
        
        // Logger
        services.AddLogging();

        // Seq
        services.AddSeqLogging(configuration);

        services.AddControllers();
        
        // HealthCheck
        services.AddHealthChecks();
        
        // Services
        services.AddServices(configuration);
        
        // RabbitMQ
        services.AddRabbitMq(configuration);
        services.AddHostedService<MetricConsumerService>();
        
        // InfluxDB
        services.AddInfluxDb(configuration);
        
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseCorsPolicy();
        
        app.UseRouting();

        return app;
    }
}