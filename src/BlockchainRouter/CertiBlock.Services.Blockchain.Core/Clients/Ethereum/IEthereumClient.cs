using CertiBlock.Services.Blockchain.Core.Events;

namespace CertiBlock.Services.Blockchain.Core.Clients.Ethereum;

public interface IEthereumClient
{
    Task RegisterCertificateAsync(CertificateRegistered certificate);
}