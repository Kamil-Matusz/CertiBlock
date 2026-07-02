using System.Security.Authentication;
using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;

namespace CertiBlock.Services.Users.Application.Handlers;

public sealed class ChangeUserPasswordHandler(IPasswordManager passwordManager, IUserRepository userRepository)
    : ICommandHandler<ChangeUserPassword>
{
    public async Task HandlerAsync(ChangeUserPassword command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        if (!passwordManager.Validate(command.CurrentPassword, user.Password))
        {
            throw new InvalidCredentialException();
        }

        var securedPassword = passwordManager.Secure(command.NewPassword);
        await userRepository.ChangeUserPassword(command.UserId, securedPassword);
    }
}