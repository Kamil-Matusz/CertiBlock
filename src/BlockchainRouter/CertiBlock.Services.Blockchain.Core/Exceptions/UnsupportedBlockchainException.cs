using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Blockchain.Core.Exceptions;

public class UnsupportedBlockchainException(Shared.Enums.Blockchain blockchain) 
    : CustomException($"Blockchain '{blockchain}' is not supported for certificate registration.")
{
    public Shared.Enums.Blockchain Blockchain { get; set; } = blockchain;
}