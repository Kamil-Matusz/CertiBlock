namespace CertiBlock.Services.Blockchain.Core.MassTransit;

public class MassTransitOptions
{
    public string Host { get; set; }
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; }
    public string Password { get; set; }
    public string Queue { get; set; }
}