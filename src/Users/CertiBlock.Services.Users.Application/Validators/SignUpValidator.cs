using CertiBlock.Services.Users.Application.Commands;
using FluentValidation;

namespace CertiBlock.Services.Users.Application.Validators;

public class SignUpValidator : AbstractValidator<SignUp>
{
    public SignUpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(100).WithMessage("Email cannot be longer than 100 characters.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(200).WithMessage("Password cannot be longer than 200 characters.");
    }
}
