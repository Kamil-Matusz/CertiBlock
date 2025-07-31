using CertiBlock.Services.Certificates.Core.DTO;

namespace CertiBlock.Services.Certificates.Core.Clients;

public interface IBlockchainRouterClient
{
    Task<string> SendToBlockchainAsync(BlockchainRegisterRequest request);
}