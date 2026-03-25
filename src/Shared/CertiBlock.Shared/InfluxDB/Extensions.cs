using InfluxDB.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Shared.InfluxDB;

public static class Extensions
{
    private const string InfluxDbSectionName = "InfluxDb";

    public static IServiceCollection AddInfluxDb(this IServiceCollection services, IConfiguration configuration, bool addHealthCheck = true)
    {
        var section = configuration.GetSection(InfluxDbSectionName);
        services.Configure<InfluxDbOptions>(section);
        var options = configuration.GetOptions<InfluxDbOptions>(InfluxDbSectionName);

        services.AddSingleton(options);
        services.AddSingleton<IInfluxDBClient>(sp => InfluxDBClientFactory.Create(options.Url, options.Token));

        if (addHealthCheck)
        {
            services.AddHealthChecks()
                .AddCheck<InfluxDbHealthCheck>("influxdb", tags: new[] { "influxdb", "database" });
        }

        return services;
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);
        return options;
    }
}