using CertiBlock.Services.Certificates.Core.DTO;
using FluentValidation;

namespace CertiBlock.Services.Certificates.Core.Validators;

public class BlockchainRegisterRequestValidator : AbstractValidator<BlockchainRegisterRequest>
{
    public BlockchainRegisterRequestValidator()
    {
        RuleFor(x => x.Hash)
            .NotEmpty().WithMessage("Certificate hash is required.")
            .Matches("^[a-fA-F0-9]{64}$")
            .WithMessage("Certificate hash must be a valid 64-character SHA256 hex string.");

        RuleFor(x => x.Issuer)
            .NotEmpty().WithMessage("Issuer name is required.")
            .MaximumLength(100).WithMessage("Issuer name must not exceed 100 characters.");

        RuleFor(x => x.Blockchain)
            .NotEmpty().WithMessage("Blockchain type is required.")
            .Must(b => b.ToLower() == "polygon" || b.ToLower() == "ethereum")
            .WithMessage("Blockchain must be either 'polygon' or 'ethereum'.");
    }
}