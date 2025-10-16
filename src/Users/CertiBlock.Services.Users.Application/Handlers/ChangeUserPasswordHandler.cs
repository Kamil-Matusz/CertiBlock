using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.Repositories;

namespace CertiBlock.Services.Users.Application.Handlers;

public sealed class ChangeUserPasswordHandler(IPasswordManager passwordManager, IUserMongoRepository userRepository)
    : ICommandHandler<ChangeUserPassword>
{
    public async Task HandlerAsync(ChangeUserPassword command)
    {
        var securedPassword = passwordManager.Secure(command.Password);

        await userRepository.ChangeUserPassword(command.UserId, securedPassword);
    }
}