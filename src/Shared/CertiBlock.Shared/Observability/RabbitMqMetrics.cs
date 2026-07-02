using System.Diagnostics.Metrics;

namespace CertiBlock.Shared.Observability;

public static class RabbitMqMetrics
{
    public const string MeterName = "CertiBlock.RabbitMQ";

    private static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> MessagesPublished = Meter.CreateCounter<long>(
        "rabbitmq.messages.published",
        description: "Number of messages published to RabbitMQ");

    public static readonly Counter<long> MessagesConsumed = Meter.CreateCounter<long>(
        "rabbitmq.messages.consumed",
        description: "Number of messages consumed from RabbitMQ");

    public static readonly Histogram<double> PublishDuration = Meter.CreateHistogram<double>(
        "rabbitmq.publish.duration",
        unit: "ms",
        description: "Duration of message publish operations in milliseconds");

    public static readonly Histogram<double> ConsumeDuration = Meter.CreateHistogram<double>(
        "rabbitmq.consume.duration",
        unit: "ms",
        description: "Duration of message consume processing in milliseconds");
}