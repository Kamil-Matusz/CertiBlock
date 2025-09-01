using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Ethereum.Core.Exceptions;

public class EthereumTransactionsNotFoundException : CustomException
{
    public Guid? Id { get; set; }
    public string? StringId { get; set; }
    public Guid? CertificateId { get; set; }

    public EthereumTransactionsNotFoundException(Guid id) 
        : base($"Eth transaction with ID: '{id}' was not found.")
    {
        Id = id;
    }

    public EthereumTransactionsNotFoundException(string id) 
        : base($"Eth transaction with ID: '{id}' was not found.")
    {
        StringId = id;
    }
}