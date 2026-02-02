using CertiBlock.Shared.DTO;
using FluentValidation;

namespace CertiBlock.Services.Polygon.Core.Validators;

public class MetricDtoValidator : AbstractValidator<MetricDto>
{
    public MetricDtoValidator()
    {
        RuleFor(x => x.CertificateId)
            .NotEmpty()
            .WithMessage("Certificate ID is required");
        
        RuleFor(x => x.TransactionHash)
            .NotEmpty()
            .WithMessage("Transaction Hash ID is required");
    }
}