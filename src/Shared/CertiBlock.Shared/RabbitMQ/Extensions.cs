using CertiBlock.Shared.RabbitMQ.Consumer;
using CertiBlock.Shared.RabbitMQ.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace CertiBlock.Shared.RabbitMQ;

public static class Extensions
{
    private const string RabbitMqSectionName = "RabbitMq";

    public static IServiceCollection AddRabbitMqConnection(this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(RabbitMqSectionName);
        services.Configure<RabbitMqOptions>(section);
        var options = configuration.GetOptions<RabbitMqOptions>(RabbitMqSectionName);

        services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.Username,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        });

        return services;
    }
    
    private static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);
        return options;
    }
    
    public static IServiceCollection AddRabbitMqProducer<TProducer>(this IServiceCollection services, string queueName,
        bool declareQueue = true) where TProducer : class, IRabbitMqProducer
    {
        services.AddSingleton<TProducer>(sp =>
        {
            var factory = sp.GetRequiredService<IConnectionFactory>();
            var connection = factory.CreateConnection($"{typeof(TProducer).Name}-Connection");
            var channel = connection.CreateModel();
    
            if (declareQueue)
            {
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            }

            var producer = (TProducer)Activator.CreateInstance(typeof(TProducer), connection, channel, queueName)!;
            
            return producer;
        });

        return services;
    }
    
    public static IServiceCollection AddRabbitMqConsumer<TConsumer>(
        this IServiceCollection services)
        where TConsumer : BackgroundService, IRabbitMqConsumer
    {
        services.AddHostedService<TConsumer>();
        return services;
    }
}