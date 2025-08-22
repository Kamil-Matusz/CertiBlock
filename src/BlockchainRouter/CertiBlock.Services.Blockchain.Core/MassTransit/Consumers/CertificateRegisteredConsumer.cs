using CertiBlock.Services.Blockchain.Core.Clients.Ethereum;
using CertiBlock.Services.Blockchain.Core.Clients.Polygon;
using CertiBlock.Services.Blockchain.Core.Events;
using CertiBlock.Services.Blockchain.Core.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Blockchain.Core.MassTransit.Consumers;

public class CertificateRegisteredConsumer(
    ILogger<CertificateRegisteredConsumer> logger, 
    IEthereumClient ethereumClient, 
    IPolygonClient polygonClient)
    : IConsumer<CertificateRegistered>
{
    public async Task Consume(ConsumeContext<CertificateRegistered> context)
    {
        var message = context.Message;
        
        logger.LogInformation(
            "Przetwarzanie certyfikatu - CertificateId: {CertificateId}, Hash: {Hash}, Issuer: {Issuer}, Blockchain: {Blockchain}",
            message.CertificateId,
            message.CertificateHash,
            message.Issuer,
            message.Blockchain);

        try
        {
            await RegisterCertificateOnBlockchain(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Błąd podczas rejestracji certyfikatu {CertificateId} na blockchain {Blockchain}",
                message.CertificateId,
                message.Blockchain);
            throw;
        }
    }

    private Task RegisterCertificateOnBlockchain(CertificateRegistered certificate)
    {
        return certificate.Blockchain switch
        {
            Shared.Enums.Blockchain.Ethereum => ethereumClient.RegisterCertificateAsync(certificate),
            Shared.Enums.Blockchain.Polygon => polygonClient.RegisterCertificateAsync(certificate),
            _ => throw new UnsupportedBlockchainException(certificate.Blockchain)
        };
    }
}