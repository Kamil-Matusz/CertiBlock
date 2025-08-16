using CertiBlock.Services.Blockchain.Core.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Services.Blockchain.Core.MassTransit.Consumers;

public class CertificateRegisteredConsumer(ILogger<CertificateRegisteredConsumer> logger)
    : IConsumer<CertificateRegistered>
{
    public Task Consume(ConsumeContext<CertificateRegistered> context)
    {
        var message = context.Message;
        
        logger.LogError(
            "Otrzymano wiadomość CertificateRegistered - CertificateId: {CertificateId}, Hash: {Hash}, Issuer: {Issuer}, Blockchain: {Blockchain}",
            message.CertificateId,
            message.CertificateHash,
            message.Issuer,
            message.Blockchain);

        return Task.CompletedTask;
    }
}