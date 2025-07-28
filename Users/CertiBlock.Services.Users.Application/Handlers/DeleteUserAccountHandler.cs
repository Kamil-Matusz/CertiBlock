using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;

namespace CertiBlock.Services.Users.Application.Handlers;

internal sealed class DeleteUserAccountHandler : ICommandHandler<DeleteUserAccount>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserAccountHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandlerAsync(DeleteUserAccount command)
    {
        var user = await _userRepository.GetUserByIdAsync(command.UserId);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await _userRepository.DeleteUserAsync(user);
    }
}