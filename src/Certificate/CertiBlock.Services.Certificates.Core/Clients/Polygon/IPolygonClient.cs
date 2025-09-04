using CertiBlock.Services.Certificates.Core.Events;

namespace CertiBlock.Services.Certificates.Core.Clients.Polygon;

public interface IPolygonClient
{
    Task RegisterCertificateAsync(CertificateRegistered certificate);
}