using RabbitMQ.Client;

namespace CertiBlock.Shared.RabbitMQ;

public static class RabbitMqTopology
{
    public const string DeadLetterExchange = "certiblock.dlx";
    public const string DeadLetterQueueSuffix = ".dlq";

    public static void Declare(IModel channel, params string[] queues)
    {
        channel.ExchangeDeclare(DeadLetterExchange, ExchangeType.Direct, durable: true, autoDelete: false);

        foreach (var queue in queues)
        {
            var deadLetterQueue = queue + DeadLetterQueueSuffix;
            channel.QueueDeclare(deadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(deadLetterQueue, DeadLetterExchange, routingKey: queue);

            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object> { ["x-dead-letter-exchange"] = DeadLetterExchange });
        }
    }
}