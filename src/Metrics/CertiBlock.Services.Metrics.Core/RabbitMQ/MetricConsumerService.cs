using System.Text;
using System.Text.Json;
using CertiBlock.Shared.Messaging;
using CertiBlock.Shared.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertiBlock.Services.Metrics.Core.RabbitMQ;

public class MetricConsumerService(IConnection connection, IOptions<RabbitMqOptions> options, ILogger<MetricConsumerService> logger)
    : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;
    private IModel? _channel;
    private readonly string[] _queues = { "certiblock.metrics.ethereum", "certiblock.metrics.polygon" };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForConnectionAsync(stoppingToken);

        _channel = connection.CreateModel();
        _channel.BasicQos(0, 10, false);
        
        if (_options.CreateTopology)
        {
            EnsureTopology(_channel);
        }

        foreach (var queue in _queues)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.Span);
                    var evt = JsonSerializer.Deserialize<MetricCollectedEvent>(json);
                    
                    if (evt != null)
                    {
                        await ProcessMetricAsync(evt, stoppingToken);
                        
                        logger.LogInformation("Processed metric: {Blockchain} - {Operation} - {Fee}",
                            evt.Blockchain, evt.Operation, evt.TransactionFee);
                    }
                    else
                    {
                        logger.LogWarning("Received null event from queue {Queue}", queue);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "JSON deserialization error for message from {Queue}", queue);
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing message from {Queue}", queue);
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
            logger.LogInformation("Listening on queue: {Queue}", queue);
        }
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private void EnsureTopology(IModel channel)
    {
        foreach (var queue in _queues)
        {
            channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }
        
        logger.LogInformation("RabbitMQ topology created");
    }

    private async Task ProcessMetricAsync(MetricCollectedEvent evt, CancellationToken ct)
    {
        await Task.CompletedTask;
    }

    private async Task WaitForConnectionAsync(CancellationToken ct)
    {
        while (!connection.IsOpen && !ct.IsCancellationRequested)
        {
            logger.LogWarning("Waiting for RabbitMQ connection...");
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping MetricConsumerService...");
        
        _channel?.Close();
        _channel?.Dispose();
        
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}