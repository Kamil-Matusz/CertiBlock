using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application.Hangfire;

public static class Extensions
{
    public static IServiceCollection AddHangfireJobs(this IServiceCollection services)
    {
        services.AddScoped<UpdateEthereumTransactionStatusJob>();
        services.AddScoped<FetchMissingEthereumMetricsJob>();
        services.AddScoped<CheckEthereumFinalizationJob>();
        
        return services;
    }
    
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<UpdateEthereumTransactionStatusJob>(
            "update-ethereum-transaction-status",
            job => job.UpdateSubmittedTransactionsAsync(),
            Cron.HourInterval(1));
        
        recurringJobManager.AddOrUpdate<FetchMissingEthereumMetricsJob>(
            "fetch-missing-ethereum-metrics",
            job => job.FetchMissingMetricsAsync(),
            Cron.HourInterval(2));

        recurringJobManager.AddOrUpdate<CheckEthereumFinalizationJob>(
            "check-ethereum-finalization",
            job => job.CheckFinalizationAsync(),
            Cron.MinuteInterval(5));

        return app;
    }
}