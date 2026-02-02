using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Ethereum.Core.Exceptions;

public class EthereumTransactionsByCertificateIdNotFoundException(Guid certificateId)
    : CustomException($"Eth transaction with ID: '{certificateId}' was not found.")
{
    public Guid? CertificateId { get; set; } = certificateId;
}