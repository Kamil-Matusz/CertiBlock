using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CertiBlock.Shared.Messaging;
using CertiBlock.Shared.Observability;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CertiBlock.Shared.RabbitMQ;

public abstract class MetricPublisherBase : IDisposable
{
    private readonly ILogger _logger;
    private readonly IModel _channel;
    private readonly string _metricQueue;
    private readonly string _finalizationQueue;
    private static readonly ActivitySource ActivitySource = new(MessagingActivitySources.MessagingPublishSourceName);

    protected MetricPublisherBase(IConnection connection, ILogger logger, string metricQueue, string finalizationQueue)
    {
        _logger = logger;
        _metricQueue = metricQueue;
        _finalizationQueue = finalizationQueue;
        _channel = connection.CreateModel();
        _channel.ConfirmSelect();
        RabbitMqTopology.Declare(_channel, metricQueue, finalizationQueue);
    }

    public void Publish(MetricCollectedEvent metric)
        => Publish(metric, _metricQueue, metric.CertificateId);

    public void Publish(MetricFinalizedEvent metric)
        => Publish(metric, _finalizationQueue, metric.CertificateId);

    private void Publish<T>(T message, string queue, Guid certificateId)
    {
        using var activity = ActivitySource.StartActivity($"Publish {typeof(T).Name}", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", queue);
        activity?.SetTag("messaging.operation", "publish");

        var sw = Stopwatch.StartNew();
        try
        {
            PublishInternal(message, queue);

            RabbitMqMetrics.MessagesPublished.Add(1,
                new KeyValuePair<string, object?>("queue", queue),
                new KeyValuePair<string, object?>("status", "success"));
            RabbitMqMetrics.PublishDuration.Record(sw.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("queue", queue));

            _logger.LogInformation("Published {EventType} for certificate {CertificateId}", typeof(T).Name, certificateId);
        }
        catch (Exception ex)
        {
            RabbitMqMetrics.MessagesPublished.Add(1,
                new KeyValuePair<string, object?>("queue", queue),
                new KeyValuePair<string, object?>("status", "failure"));

            _logger.LogError(ex, "Failed to publish {EventType} for certificate {CertificateId}", typeof(T).Name, certificateId);
            throw;
        }
    }

    private void PublishInternal<T>(T message, string routingKey)
    {
        var json = JsonSerializer.Serialize(message, Serialization.SerializationOptions.Default);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel.CreateBasicProperties();
        props.Persistent = true;
        props.ContentType = "application/json";
        props.DeliveryMode = 2;

        props.Headers ??= new Dictionary<string, object>();
        if (Activity.Current != null)
        {
            props.Headers["traceparent"] = Activity.Current.Id;
        }

        _channel.BasicPublish(
            exchange: "",
            routingKey: routingKey,
            basicProperties: props,
            body: body);

        _channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
    }
}