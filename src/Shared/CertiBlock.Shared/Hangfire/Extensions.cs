using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace CertiBlock.Shared.Hangfire;

public static class Extensions
{
    private const string HangfireSectionName = "Hangfire";

    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<HangfireOptions>(HangfireSectionName);
        var mongoClient = new MongoClient(options.ConnectionString);

        services.AddHangfire(config => config
            .UseMongoStorage(mongoClient, options.Database, new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new MigrateMongoMigrationStrategy()
                },
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
            })
        );

        services.AddHangfireServer();

        return services;
    }
    
    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = Array.Empty<IDashboardAuthorizationFilter>()
        });

        return app;
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) 
        where T : class, new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}