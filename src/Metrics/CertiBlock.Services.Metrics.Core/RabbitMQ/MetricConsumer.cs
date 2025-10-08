using System.Text;
using System.Text.Json;
using CertiBlock.Shared.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertiBlock.Services.Metrics.Core.RabbitMQ;

public class MetricConsumer(IModel channel, ILogger<MetricConsumer> logger)
{
    public void Start()
    {
        StartConsumer("certiblock.metrics.ethereum");
        StartConsumer("certiblock.metrics.polygon");
    }

    private void StartConsumer(string queueName)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var metric = JsonSerializer.Deserialize<MetricCollectedEvent>(json);

            if (metric != null)
            {
                logger.LogInformation($"[Consumer] Received {queueName} metric {metric.CertificateId}");
            }

            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer);
        logger.LogInformation($"[Consumer] Listening on {queueName}...");
    }
}