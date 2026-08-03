using CertiBlock.Services.Polygon.Application.Hangfire;
using CertiBlock.Services.Polygon.Core.Entities;
using CertiBlock.Services.Polygon.Core.Exceptions;
using CertiBlock.Services.Polygon.Infrastructure.Configurations;
using CertiBlock.Services.Polygon.Infrastructure.DAL;
using CertiBlock.Shared.CoinGecko;
using CertiBlock.Shared.CORS;
using CertiBlock.Shared.Exceptions;
using CertiBlock.Shared.Hangfire;
using CertiBlock.Shared.Logging;
using CertiBlock.Shared.Mongo;
using CertiBlock.Shared.Observability;
using CertiBlock.Shared.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace CertiBlock.Services.Polygon.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // CORS
        services.AddCorsPolicy();
        
        // Logger
        services.AddLogging();
        
        // Seq
        services.AddSeqLogging(configuration);

        // MongoDB
        services.AddMongo(configuration);
        ConfigureMongoDbMappings();
        
        // Hangfire
        services.AddHangfire(configuration);
        
        // CoinGecko
        services.AddCoinGecko(configuration);

        // Repositories
        services.AddRepositories();
        
        services.AddControllers();
        
        // HealthCheck
        services.AddHealthChecks();
        
        // RabbitMQ
        services.AddRabbitMq(configuration);

        // Error Handling
        services.AddErrorHandling();
        services.AddSingleton<IExceptionMapper, PolygonExceptionMapper>();
        
        // OpenTelemetry
        services.AddObservability(configuration);

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseCorsPolicy();

        app.UseErrorHandling();
        app.UseRouting();
        
        app.UseHangfireDashboard();
        
        app.UseHangfireJobs();
        
        return app;
    }

    private static void ConfigureMongoDbMappings()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(PolygonMetrics)))
        {
            PolygonMetricsConfiguration.Configure();
        }
        
        if (!BsonClassMap.IsClassMapRegistered(typeof(BlockchainTransaction)))
        {
            BlockchainTransactionConfiguration.Configure();
        }
    }
}