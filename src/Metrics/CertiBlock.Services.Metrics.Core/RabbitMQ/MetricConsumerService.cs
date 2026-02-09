using System.Text;
using System.Text.Json;
using CertiBlock.Services.Metrics.Core.Services;
using CertiBlock.Shared.Messaging;
using CertiBlock.Shared.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertiBlock.Services.Metrics.Core.RabbitMQ;

public class MetricConsumerService(IConnection connection, IOptions<RabbitMqOptions> options, ILogger<MetricConsumerService> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;
    private IModel? _channel;
    private readonly string[] _metricQueues = { "certiblock.metrics.ethereum", "certiblock.metrics.polygon" };
    private readonly string[] _finalizationQueues = { "certiblock.finalization.ethereum", "certiblock.finalization.polygon" };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForConnectionAsync(stoppingToken);

        _channel = connection.CreateModel();
        _channel.BasicQos(0, 10, false);

        if (_options.CreateTopology)
        {
            EnsureTopology(_channel);
        }

        foreach (var queue in _metricQueues)
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

                        logger.LogInformation("Processed metric: {Blockchain} - {Operation} - Gas: {Gas}, CostUsd: {CostUsd}",
                            evt.Blockchain, evt.Operation, evt.GasUsed, evt.TransactionCostUsd);
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

        foreach (var queue in _finalizationQueues)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.Span);
                    var evt = JsonSerializer.Deserialize<MetricFinalizedEvent>(json);

                    if (evt != null)
                    {
                        await ProcessFinalizationAsync(evt, stoppingToken);

                        logger.LogInformation("Processed finalization: {Blockchain} - FinalizationTime: {Time}s, Confirmations: {Confirmations}",
                            evt.Blockchain, evt.FinalizationTimeSeconds, evt.Confirmations);
                    }
                    else
                    {
                        logger.LogWarning("Received null finalization event from queue {Queue}", queue);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "JSON deserialization error for finalization message from {Queue}", queue);
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing finalization message from {Queue}", queue);
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
        foreach (var queue in _metricQueues.Concat(_finalizationQueues))
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
        using var scope = serviceProvider.CreateScope();
        var metricsService = scope.ServiceProvider.GetRequiredService<IMetricsService>();
        await metricsService.WriteBlockchainMetricAsync(evt);
    }

    private async Task ProcessFinalizationAsync(MetricFinalizedEvent evt, CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var metricsService = scope.ServiceProvider.GetRequiredService<IMetricsService>();
        await metricsService.WriteFinalizationMetricAsync(evt);
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