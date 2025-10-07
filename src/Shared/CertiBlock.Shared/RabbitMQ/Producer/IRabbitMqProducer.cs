namespace CertiBlock.Shared.RabbitMQ.Producer;

public interface IRabbitMqProducer
{
    void Publish(string message);
}