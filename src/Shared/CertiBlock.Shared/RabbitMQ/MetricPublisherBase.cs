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
    {
        using var activity = ActivitySource.StartActivity("Publish MetricCollectedEvent", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", _metricQueue);
        activity?.SetTag("messaging.operation", "publish");

        var sw = Stopwatch.StartNew();
        try
        {
            PublishInternal(metric, _metricQueue);

            RabbitMqMetrics.MessagesPublished.Add(1,
                new KeyValuePair<string, object?>("queue", _metricQueue),
                new KeyValuePair<string, object?>("status", "success"));
            RabbitMqMetrics.PublishDuration.Record(sw.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("queue", _metricQueue));

            _logger.LogInformation("Published metric for certificate {CertificateId}", metric.CertificateId);
        }
        catch (Exception ex)
        {
            RabbitMqMetrics.MessagesPublished.Add(1,
                new KeyValuePair<string, object?>("queue", _metricQueue),
                new KeyValuePair<string, object?>("status", "failure"));

            _logger.LogError(ex, "Failed to publish metric for certificate {CertificateId}", metric.CertificateId);
            throw;
        }
    }

    public void Publish(MetricFinalizedEvent metric)
    {
        using var activity = ActivitySource.StartActivity("Publish MetricFinalizedEvent", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", _finalizationQueue);
        activity?.SetTag("messaging.operation", "publish");

        var sw = Stopwatch.StartNew();
        try
        {
            PublishInternal(metric, _finalizationQueue);

            RabbitMqMetrics.MessagesPublished.Add(1,
                new KeyValuePair<string, object?>("queue", _finalizationQueue),
                new KeyValuePair<string, object?>("status", "success"));
            RabbitMqMetrics.PublishDuration.Record(sw.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("queue", _finalizationQueue));

            _logger.LogInformation("Published finalization metric for certificate {CertificateId}", metric.CertificateId);
        }
        catch (Exception ex)
        {
            RabbitMqMetrics.MessagesPublished.Add(1,
                new KeyValuePair<string, object?>("queue", _finalizationQueue),
                new KeyValuePair<string, object?>("status", "failure"));

            _logger.LogError(ex, "Failed to publish finalization metric for certificate {CertificateId}", metric.CertificateId);
            throw;
        }
    }

    private void PublishInternal<T>(T message, string routingKey)
    {
        var json = JsonSerializer.Serialize(message);
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