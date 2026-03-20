using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CertiBlock.Shared.Observability;

public static class Extensions
{
    private const string ObservabilitySectionName = "Observability";

    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(ObservabilitySectionName);
        services.Configure<ObservabilityOptions>(section);

        var options = configuration.GetOptions<ObservabilityOptions>(ObservabilitySectionName);

        if (!options.Enabled)
        {
            return services;
        }

        var serviceName = configuration.GetValue<string>("App:AppName") ?? "UnknownService";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            // (TRACING -> JAEGER)
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(
                        MessagingActivitySources.DefaultSourceName,
                        MessagingActivitySources.MessagingPublishSourceName,
                        MessagingActivitySources.MessagingConsumeSourceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri(options.Endpoint);
                    });
            })
            // (METRICS -> PROMETHEUS)
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(MessagingActivitySources.DefaultSourceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });

        return services;
    }
    
    public static WebApplication UseObservability(this WebApplication app)
    {
        app.MapPrometheusScrapingEndpoint();
    
        return app;
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}