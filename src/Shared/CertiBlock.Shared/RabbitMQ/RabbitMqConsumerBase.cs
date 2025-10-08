using System.Text;
using CertiBlock.Shared.RabbitMQ.Consumer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertiBlock.Shared.RabbitMQ;

public abstract class RabbitMqConsumerBase(IConnectionFactory connectionFactory, ILogger logger)
    : BackgroundService, IRabbitMqConsumer
{
    private IConnection? _connection;
    private readonly List<IModel> _channels = new();
    
    protected abstract string[] QueueNames { get; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = connectionFactory.CreateConnection($"{GetType().Name}-Connection");

        foreach (var queueName in QueueNames)
        {
            var channel = _connection.CreateModel();
            _channels.Add(channel);
            
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            var channelLocal = channel; // Capture dla closure
            var queueNameLocal = queueName; // Capture dla closure
            
            consumer.Received += async (model, ea) =>
            {
                var deliveryTag = ea.DeliveryTag;
                
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    await HandleMessage(queueNameLocal, message, stoppingToken);
                    
                    // Każdy channel ma swoje wiadomości - thread-safe
                    channelLocal.BasicAck(deliveryTag: deliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing message from queue {QueueName}", queueNameLocal);
                    channelLocal.BasicNack(deliveryTag: deliveryTag, multiple: false, requeue: false);
                }
            };

            channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer);
            
            logger.LogInformation("Started consuming from queue: {QueueName}", queueName);
        }

        return Task.CompletedTask;
    }

    protected abstract Task HandleMessage(string queueName, string message, CancellationToken cancellationToken);

    public override void Dispose()
    {
        foreach (var channel in _channels)
        {
            try
            {
                channel?.Close();
                channel?.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error closing channel");
            }
        }

        try
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error closing connection");
        }

        base.Dispose();
    }
}