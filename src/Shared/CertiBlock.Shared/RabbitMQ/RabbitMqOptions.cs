namespace CertiBlock.Shared.RabbitMQ;

public class RabbitMqOptions
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; } = "/";
    public bool CreateTopology { get; set; }
}