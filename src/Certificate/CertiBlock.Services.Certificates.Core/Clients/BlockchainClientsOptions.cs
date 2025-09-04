namespace CertiBlock.Services.Certificates.Core.Clients;

public class BlockchainClientsOptions
{
    public string Ethereum { get; set; }
    public string Polygon { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}