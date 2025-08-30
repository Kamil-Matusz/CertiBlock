using CertiBlock.Services.Ethereum.Core.DTO;
using FluentValidation;

namespace CertiBlock.Services.Ethereum.Core.Validators;

public class BlockchainTransactionDtoValidator : AbstractValidator<BlockchainTransactionDto>
{
    public BlockchainTransactionDtoValidator()
    {
        RuleFor(x => x.CertificateId)
            .NotEmpty()
            .WithMessage("Certificate ID is required");

        RuleFor(x => x.CertificateHash)
            .NotEmpty()
            .WithMessage("Certificate hash is required")
            .Length(64, 66)
            .WithMessage("Certificate hash must be 64 or 66 characters long")
            .Must(BeValidHash)
            .WithMessage("Certificate hash must be a valid hexadecimal string (with or without 0x prefix)");

        RuleFor(x => x.Blockchain)
            .IsInEnum()
            .WithMessage("Invalid blockchain type");

        /*RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status");*/
        
        /*RuleFor(x => x.TransactionHash)
            .Must(BeValidEthereumTransactionHash)
            .When(x => !string.IsNullOrWhiteSpace(x.TransactionHash))
            .WithMessage("Transaction hash must be a valid Ethereum transaction hash (66 characters starting with 0x)");*/
    }
    
    private static bool BeValidHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return false;
        
        var cleanHash = hash.StartsWith("0x", StringComparison.OrdinalIgnoreCase) 
            ? hash.Substring(2) 
            : hash;
        
        return cleanHash.Length == 64 && 
               cleanHash.All(c => char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
    }
    
    private static bool BeValidEthereumTransactionHash(string transactionHash)
    {
        if (string.IsNullOrWhiteSpace(transactionHash))
            return true;
        
        return transactionHash.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
               transactionHash.Length == 66 &&
               transactionHash.Substring(2).All(c => char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
    }

}