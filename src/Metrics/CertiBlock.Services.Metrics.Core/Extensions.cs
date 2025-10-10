using CertiBlock.Services.Metrics.Core.RabbitMQ;
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
        // Logger
        services.AddLogging();

        // Seq
        services.AddSeqLogging(configuration);

        services.AddControllers();
        
        // HealthCheck
        services.AddHealthChecks();
        
        // RabbitMQ
        services.AddRabbitMq(configuration);
        services.AddHostedService<MetricConsumerService>();
        
        // InfluxDB
        services.AddInfluxDb(configuration);
        
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseRouting();

        return app;
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);

        return options;
    }
}