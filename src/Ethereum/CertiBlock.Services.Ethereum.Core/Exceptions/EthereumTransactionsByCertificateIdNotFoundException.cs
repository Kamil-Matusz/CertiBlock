using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Ethereum.Core.Exceptions;

public class EthereumTransactionsByCertificateIdNotFoundException : CustomException
{
    public Guid? CertificateId { get; set; }

    public EthereumTransactionsByCertificateIdNotFoundException(Guid certificateId) 
        : base($"Eth transaction with ID: '{certificateId}' was not found.")
    {
        CertificateId = certificateId;
    }
}