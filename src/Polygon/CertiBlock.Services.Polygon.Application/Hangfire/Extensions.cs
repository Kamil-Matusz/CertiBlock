using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Application.Hangfire;

public static class Extensions
{
    public static IServiceCollection AddHangfireJobs(this IServiceCollection services)
    {
        services.AddScoped<UpdatePolygonTransactionStatusJob>();
        services.AddScoped<FetchMissingPolygonMetricsJob>();
        services.AddScoped<CheckPolygonFinalizationJob>();
        
        return services;
    }
    
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<UpdatePolygonTransactionStatusJob>(
            "update-polygon-transaction-status",
            job => job.UpdateSubmittedTransactionsAsync(),
            Cron.HourInterval(1));
        
        recurringJobManager.AddOrUpdate<FetchMissingPolygonMetricsJob>(
            "fetch-missing-polygon-metrics",
            job => job.FetchMissingMetricsAsync(),
            Cron.HourInterval(2));

        recurringJobManager.AddOrUpdate<CheckPolygonFinalizationJob>(
            "check-polygon-finalization",
            job => job.CheckFinalizationAsync(),
            Cron.MinuteInterval(5));

        return app;
    }
}