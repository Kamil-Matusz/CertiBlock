using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Blockchain.Core.Exceptions;

public class BlockchainConfigurationException : CustomException
{
    public string ClientName { get; }
    public string ConfigurationKey { get; }

    public BlockchainConfigurationException(string clientName, string configurationKey)
        : base($"Missing configuration for blockchain client '{clientName}'. Configuration key '{configurationKey}' is required but was not found or is empty.")
    {
        ClientName = clientName;
        ConfigurationKey = configurationKey;
    }
}