using CertiBlock.Services.Polygon.Core.DTO;
using FluentValidation;

namespace CertiBlock.Services.Polygon.Core.Validators;

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
        
        RuleFor(x => x.Issuer)
            .NotEmpty()
            .WithMessage("Issuer ID is required");
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
}