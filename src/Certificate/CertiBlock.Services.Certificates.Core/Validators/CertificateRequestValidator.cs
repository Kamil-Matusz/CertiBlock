using CertiBlock.Services.Certificates.Core.DTO;
using FluentValidation;

namespace CertiBlock.Services.Certificates.Core.Validators;

public class CertificateRequestValidator : AbstractValidator<CertificateRequest>
{
    public CertificateRequestValidator()
    {
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required.")
            .MaximumLength(100).WithMessage("Owner name must not exceed 100 characters.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Certificate title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.IssuedBy)
            .NotEmpty().WithMessage("Issuing institution is required.")
            .MaximumLength(100).WithMessage("Institution name must not exceed 100 characters.");

        RuleFor(x => x.IssuedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Issue date cannot be in the future.");

        RuleFor(x => x.Blockchain)
            .NotEmpty().WithMessage("Blockchain type is required.")
            .IsInEnum().WithMessage("Invalid blockchain type. Supported: Polygon, Ethereum.");
    }
}