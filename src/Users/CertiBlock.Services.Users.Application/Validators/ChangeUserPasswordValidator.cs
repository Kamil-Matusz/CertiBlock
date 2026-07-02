using CertiBlock.Services.Users.Application.Commands;
using FluentValidation;

namespace CertiBlock.Services.Users.Application.Validators;

public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPassword>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(200).WithMessage("Password cannot be longer than 200 characters.");
    }
}