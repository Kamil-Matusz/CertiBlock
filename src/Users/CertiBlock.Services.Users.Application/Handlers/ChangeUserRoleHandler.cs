using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;

namespace CertiBlock.Services.Users.Application.Handlers;

public sealed class ChangeUserRoleHandler(IUserRepository userRepository) : ICommandHandler<ChangeUserRole>
{
    public async Task HandlerAsync(ChangeUserRole command)
    {
        var userId = command.UserId;
        var role = string.IsNullOrWhiteSpace(command.Role) ? Role.User() : new Role(command.Role);
        if (role == "Admin" || role == "User")
        {
            await userRepository.ChangeUserRoleAsync(userId, role);    
        }
        else
        {
            throw new UserRoleNotExistException();
        }
    }
}