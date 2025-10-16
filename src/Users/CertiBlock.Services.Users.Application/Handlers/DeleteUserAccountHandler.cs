using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;

namespace CertiBlock.Services.Users.Application.Handlers;

public sealed class DeleteUserAccountHandler(IUserMongoRepository userRepository) : ICommandHandler<DeleteUserAccount>
{
    public async Task HandlerAsync(DeleteUserAccount command)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await userRepository.DeleteUserAsync(user);
    }
}