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
    public async Task HandleAsync(SignIn command)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetUserByEmailAsync(email);
        if (user is null)
        {
            throw new InvalidCredentialException();
        }

        if (!passwordManager.Validate(command.Password, user.Password))
        {
            throw new InvalidCredentialException();
        }

        if (!user.IsActive)
        {
            throw new AccountIsNotActiveException(email);
        }

        var jwt = authenticator.CreateToken(user.UserId, user.Role);
        tokenStorage.SetToken(jwt);
    }
}