using CertiBlock.Services.Polygon.Core.DTO;
using FluentValidation;

namespace CertiBlock.Services.Polygon.Core.Validators;

public class PolygonAddressDtoValidator : AbstractValidator<PolygonAddressDto>
{
    public PolygonAddressDtoValidator()
    {
        RuleFor(x => x.WalletAddress)
            .NotEmpty().WithMessage("Wallet Address is required.");
    }
}