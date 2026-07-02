using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Application.Services.Clock;
using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;

namespace CertiBlock.Services.Users.Application.Handlers;

public sealed class SignUpHandler(IClock clock, IPasswordManager passwordManager, IUserRepository userRepository)
    : ICommandHandler<SignUp>
{
    public async Task HandlerAsync(SignUp command)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        if (await userRepository.GetUserByEmailAsync(email) is not null)
        {
            throw new EmailAlreadyInUseException(email);
        }

        var securedPassword = passwordManager.Secure(command.Password);
        var user = new User(command.UserId, email, securedPassword, Role.User(), true, clock.CurrentDate());

        await userRepository.AddUserAsync(user);
    }
}