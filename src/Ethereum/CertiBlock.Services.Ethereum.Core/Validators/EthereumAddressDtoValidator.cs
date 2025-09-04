using CertiBlock.Services.Ethereum.Core.DTO;
using FluentValidation;

namespace CertiBlock.Services.Ethereum.Core.Validators;

public class EthereumAddressDtoValidator : AbstractValidator<EthereumAddressDto>
{
    public EthereumAddressDtoValidator()
    {
        RuleFor(x => x.WalletAddress)
            .NotEmpty().WithMessage("Wallet Address is required.");
    }
}