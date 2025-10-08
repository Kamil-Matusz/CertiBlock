using CertiBlock.Shared.RabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CertiBlock.Services.Metrics.Core.RabbitMQ;

public class MessageConsumer(IConnectionFactory connectionFactory, ILogger<MessageConsumer> logger) : RabbitMqConsumerBase(connectionFactory, logger)
{
    protected override string[] QueueNames => ["queue-ethereum", "queue-polygon"];

    protected override async Task HandleMessage(string queueName, string message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{queueName}] Processing: {message}");
        await Task.Delay(100, cancellationToken);
        Console.WriteLine($"[{queueName}] Completed: {message}");
    }
}