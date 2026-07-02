using CertiBlock.Services.Users.Application.Commands;
using FluentValidation;

namespace CertiBlock.Services.Users.Application.Validators;

public class SignInValidator : AbstractValidator<SignIn>
{
    public SignInValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
