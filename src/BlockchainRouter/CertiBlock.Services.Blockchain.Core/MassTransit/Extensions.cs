using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Blockchain.Core.MassTransit;

internal static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        var options = services.GetOptions<MassTransitOptions>("MassTransit:RabbitMq");
        services.AddSingleton(options);
        
        services.AddMassTransit(x =>
        {
            x.AddConsumers(typeof(Extensions).Assembly);
    
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(options.Host, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
        
                cfg.ReceiveEndpoint(options.Queue, e =>
                {
                    e.ConfigureConsumers(context);
                });
            });
        });
        
        services.AddMassTransitHostedService();

        return services;
    }

    private static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}