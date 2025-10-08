using System.Text;
using System.Text.Json;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertiBlock.Services.Metrics.Core;

public class MetricConsumerService(IConnection connection, IModel channel, ILogger<MetricConsumerService> logger) : BackgroundService
{
    private IModel _channel = channel;
    private readonly string[] _queues = { "certiblock.metrics.ethereum", "certiblock.metrics.polygon" };

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = connection.CreateModel();
        _channel.BasicQos(0, 1, false);

        foreach (var q in _queues)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var evt = JsonSerializer.Deserialize<MetricCollectedEvent>(json);
                    
                    if (evt != null)
                    {
                        logger.LogInformation("✅ Received Metric Event: {Blockchain} - {Operation} - {Fee}",
                            evt.Blockchain, evt.Operation, evt.TransactionFee);
                    }
                    else
                    {
                        logger.LogWarning("⚠️ Received null event payload");
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Consumer error: {ex}");
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: q, autoAck: false, consumer: consumer);
            logger.LogInformation($"Listening on {q}");
        }

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _channel?.Dispose();
        return base.StopAsync(cancellationToken);
    }
}