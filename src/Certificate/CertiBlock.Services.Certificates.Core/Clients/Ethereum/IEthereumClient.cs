using CertiBlock.Services.Certificates.Core.Events;

namespace CertiBlock.Services.Certificates.Core.Clients.Ethereum;

public interface IEthereumClient
{
    Task RegisterCertificateAsync(CertificateRegistered certificate);
}