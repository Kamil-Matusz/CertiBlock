using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CertiBlock.Services.Metrics.Core.Services.Metrics;
using CertiBlock.Shared.Messaging;
using CertiBlock.Shared.Observability;
using CertiBlock.Shared.RabbitMQ;
using CertiBlock.Shared.Serialization;
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
    private static readonly ActivitySource ActivitySource = new(MessagingActivitySources.MessagingConsumeSourceName);

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
            ConsumeQueue<MetricCollectedEvent>(queue, ProcessMetricAsync, stoppingToken);
        }

        foreach (var queue in _finalizationQueues)
        {
            ConsumeQueue<MetricFinalizedEvent>(queue, ProcessFinalizationAsync, stoppingToken);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private void ConsumeQueue<TEvent>(string queue, Func<TEvent, CancellationToken, Task> process, CancellationToken stoppingToken)
        where TEvent : class
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var parentContext = GetParentContext(ea);
                using var activity = ActivitySource.StartActivity($"Consume {typeof(TEvent).Name}", ActivityKind.Consumer, parentContext);
                activity?.SetTag("messaging.system", "rabbitmq");
                activity?.SetTag("messaging.destination", queue);
                activity?.SetTag("messaging.operation", "receive");

                var json = Encoding.UTF8.GetString(ea.Body.Span);
                var evt = JsonSerializer.Deserialize<TEvent>(json, SerializationOptions.Default);

                if (evt != null)
                {
                    await process(evt, stoppingToken);
                    logger.LogInformation("Processed {EventType} from {Queue}", typeof(TEvent).Name, queue);
                }
                else
                {
                    logger.LogWarning("Received null event from queue {Queue}", queue);
                }

                _channel.BasicAck(ea.DeliveryTag, false);

                RabbitMqMetrics.MessagesConsumed.Add(1,
                    new KeyValuePair<string, object?>("queue", queue),
                    new KeyValuePair<string, object?>("status", "success"));
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "JSON deserialization error for message from {Queue}", queue);
                _channel.BasicNack(ea.DeliveryTag, false, requeue: false);

                RabbitMqMetrics.MessagesConsumed.Add(1,
                    new KeyValuePair<string, object?>("queue", queue),
                    new KeyValuePair<string, object?>("status", "deserialization_error"));
            }
            catch (Exception ex)
            {
                var requeue = !ea.Redelivered;
                logger.LogError(ex, "Error processing message from {Queue} (requeue: {Requeue})", queue, requeue);
                _channel.BasicNack(ea.DeliveryTag, false, requeue);

                RabbitMqMetrics.MessagesConsumed.Add(1,
                    new KeyValuePair<string, object?>("queue", queue),
                    new KeyValuePair<string, object?>("status", "failure"));
            }
            finally
            {
                RabbitMqMetrics.ConsumeDuration.Record(sw.Elapsed.TotalMilliseconds,
                    new KeyValuePair<string, object?>("queue", queue));
            }
        };

        _channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
        logger.LogInformation("Listening on queue: {Queue}", queue);
    }

    private static ActivityContext GetParentContext(BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties.Headers != null &&
            ea.BasicProperties.Headers.TryGetValue("traceparent", out var traceId) &&
            traceId is byte[] traceparentBytes)
        {
            var traceparent = Encoding.UTF8.GetString(traceparentBytes);
            if (ActivityContext.TryParse(traceparent, null, isRemote: true, out var context))
                return context;
        }

        return default;
    }

    private void EnsureTopology(IModel channel)
    {
        RabbitMqTopology.Declare(channel, _metricQueues.Concat(_finalizationQueues).ToArray());
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
