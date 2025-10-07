using System.Text;
using CertiBlock.Shared.RabbitMQ.Producer;
using RabbitMQ.Client;

namespace CertiBlock.Services.Ethereum.Infrastructure.RabbitMQ;

public class FirstProducer(IConnection connection, IModel channel, string queueName) : IRabbitMqProducer, IDisposable
{
    public void Publish(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        
        channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: properties,
            body: body);
    }

    public void Dispose()
    {
        channel?.Close();
        channel?.Dispose();
        connection?.Close();
        connection?.Dispose();
    }
}