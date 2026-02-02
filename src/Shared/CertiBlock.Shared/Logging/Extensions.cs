using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace CertiBlock.Shared.Logging;

public static class Extensions
{
    public static IServiceCollection AddSeqLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetOptions<SeqOptions>("Logging:Seq");
        services.AddSingleton(options);
    
        var logLevel = Enum.TryParse<LogEventLevel>(options.MinimumLevel, true, out var level)
            ? level
            : LogEventLevel.Information;
    
        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.WithProperty("Service", options.ServiceName)
            .Enrich.WithProperty("Environment", options.Environment)
            .WriteTo.Console()
            .WriteTo.Seq(options.ServerUrl, apiKey: options.ApiKey)
            .CreateLogger();
    
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(logger);
        });
    
        return services;
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}