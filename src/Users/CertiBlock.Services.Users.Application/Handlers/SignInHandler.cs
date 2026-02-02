using System.Security.Authentication;
using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;

namespace CertiBlock.Services.Users.Application.Handlers;

public sealed class SignInHandler(IUserRepository userRepository, IAuthenticator authenticator, IPasswordManager passwordManager,
    ITokenStorage tokenStorage) : ICommandHandler<SignIn>
{
    public async Task HandlerAsync(SignIn command)
    {
        var user = await userRepository.GetUserByEmailAsync(command.Email);
        if (user is null)
        {
            throw new InvalidCredentialException();
        }

        if (!passwordManager.Validate(command.Password, user.Password))
        {
            throw new InvalidCredentialException();
        }

        bool accountIsActive = await userRepository.CheckAccountActivity(command.Email);
        if (accountIsActive is false)
        {
            throw new AccountIsNotActiveException(command.Email);
        }

        var jwt = authenticator.CreateToken(user.UserId, user.Role);
        tokenStorage.SetToken(jwt);
    }
}