using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CertiBlock.Shared.Messaging;
using CertiBlock.Shared.Observability;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CertiBlock.Services.Ethereum.Application.RabbitMQ;

public class MetricPublisher : IDisposable
{
    private readonly ILogger<MetricPublisher> _logger;
    private readonly IModel _channel;
    private static readonly ActivitySource ActivitySource = new(MessagingActivitySources.MessagingPublishSourceName);

    public MetricPublisher(IConnection connection, ILogger<MetricPublisher> logger)
    {
        _logger = logger;
        _channel = connection.CreateModel();
        _channel.ConfirmSelect();
    }

    public void Publish(MetricCollectedEvent metric)
    {
        using var activity = ActivitySource.StartActivity("Publish MetricCollectedEvent", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", "certiblock.metrics.ethereum");
        activity?.SetTag("messaging.operation", "publish");

        try
        {
            var json = JsonSerializer.Serialize(metric);
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
                routingKey: "certiblock.metrics.ethereum",
                basicProperties: props,
                body: body
            );
            
            _channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
            _logger.LogInformation("Published Ethereum metric for certificate {CertificateId}", 
                metric.CertificateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish metric for certificate {CertificateId}", 
                metric.CertificateId);
            throw;
        }
    }

    public void Publish(MetricFinalizedEvent metric)
    {
        using var activity = ActivitySource.StartActivity("Publish MetricFinalizedEvent", ActivityKind.Producer);
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination", "certiblock.finalization.ethereum");
        activity?.SetTag("messaging.operation", "publish");

        try
        {
            var json = JsonSerializer.Serialize(metric);
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
                routingKey: "certiblock.finalization.ethereum",
                basicProperties: props,
                body: body
            );

            _channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
            _logger.LogInformation("Published Ethereum finalization metric for certificate {CertificateId}",
                                    metric.CertificateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish finalization metric for certificate {CertificateId}",
                             metric.CertificateId);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
    }
}