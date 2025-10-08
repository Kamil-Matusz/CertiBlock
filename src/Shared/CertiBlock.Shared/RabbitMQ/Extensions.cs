using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace CertiBlock.Shared.RabbitMQ;

public static class Extensions
{
    private const string RabbitSectionName = "RabbitMq";

    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(RabbitSectionName);
        services.Configure<RabbitMqOptions>(section);

        var options = configuration.GetOptions<RabbitMqOptions>(RabbitSectionName);

        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.Username,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            DispatchConsumersAsync = true
        };

        var connection = factory.CreateConnection();
        services.AddSingleton<IConnection>(connection);
        
        services.AddSingleton(sp =>
        {
            var conn = sp.GetRequiredService<IConnection>();
            var channel = conn.CreateModel();

            if (options.CreateTopology)
            {
                string[] queues =
                {
                    "certiblock.metrics.ethereum",
                    "certiblock.metrics.polygon"
                };

                foreach (var queue in queues)
                {
                    channel.QueueDeclare(
                        queue: queue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                }
            }

            return channel;
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
}