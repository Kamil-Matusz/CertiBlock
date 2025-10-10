using System.Text;
using System.Text.Json;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CertiBlock.Services.Polygon.Application.RabbitMQ;

public class MetricPublisher : IDisposable
{
    private readonly IModel _channel;
    private readonly ILogger<MetricPublisher> _logger;

    public MetricPublisher(IConnection connection, ILogger<MetricPublisher> logger)
    {
        _logger = logger;
        _channel = connection.CreateModel();
        _channel.ConfirmSelect();
    }

    public void Publish(MetricCollectedEvent metric)
    {
        try
        {
            var json = JsonSerializer.Serialize(metric);
            var body = Encoding.UTF8.GetBytes(json);

            var props = _channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";
            props.DeliveryMode = 2;

            _channel.BasicPublish(
                exchange: "",
                routingKey: "certiblock.metrics.polygon",
                basicProperties: props,
                body: body
            );

            _channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));

            _logger.LogInformation("Published Polygon metric for certificate {CertificateId}", 
                metric.CertificateId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish Polygon metric for certificate {CertificateId}", 
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