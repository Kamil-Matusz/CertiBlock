using System.Text;
using System.Text.Json;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CertiBlock.Services.Ethereum.Application.RabbitMQ;

public class MetricPublisher(IModel channel, ILogger<MetricPublisher> logger)
{
    public void Publish(MetricCollectedEvent metric)
    {
        var json = JsonSerializer.Serialize(metric);
        var body = Encoding.UTF8.GetBytes(json);

        var props = channel.CreateBasicProperties();
        props.Persistent = true;

        channel.BasicPublish(
            exchange: "",
            routingKey: "certiblock.metrics.ethereum",
            basicProperties: props,
            body: body
        );
        
        logger.LogInformation($"[Publisher] Sent Ethereum metric {metric.CertificateId}");
    }
}
