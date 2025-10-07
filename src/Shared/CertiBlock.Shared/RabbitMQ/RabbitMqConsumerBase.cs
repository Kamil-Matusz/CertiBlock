using System.Text;
using CertiBlock.Shared.RabbitMQ.Consumer;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertiBlock.Shared.RabbitMQ;

public abstract class RabbitMqConsumerBase(IConnectionFactory connectionFactory) : BackgroundService, IRabbitMqConsumer
{
    private IConnection? _connection;
    private IModel? _channel;
    protected abstract string[] QueueNames { get; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = connectionFactory.CreateConnection($"{GetType().Name}-Connection");
        _channel = _connection.CreateModel();
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        foreach (var queueName in QueueNames)
        {
            _channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    await HandleMessage(queueName, message, stoppingToken);
                    
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer);
        }

        return Task.CompletedTask;
    }

    protected abstract Task HandleMessage(string queueName, string message, CancellationToken cancellationToken);

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}