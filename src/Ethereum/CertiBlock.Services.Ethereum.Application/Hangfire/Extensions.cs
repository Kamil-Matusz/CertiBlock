using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application.Hangfire;

public static class Extensions
{
    public static IServiceCollection AddHangfireJobs(this IServiceCollection services)
    {
        services.AddScoped<UpdateEthereumTransactionStatusJob>();
        
        return services;
    }
    
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<UpdateEthereumTransactionStatusJob>(
            "update-ethereum-transaction-status",
            job => job.UpdateSubmittedTransactionsAsync(),
            Cron.HourInterval(1));
        
        return app;
    }
}