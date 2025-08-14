using CertiBlock.Services.Blockchain.Core.Events;

namespace CertiBlock.Services.Blockchain.Core.Clients.Polygon;

public interface IPolygonClient
{
    Task RegisterCertificateAsync(CertificateRegistered certificate);
}