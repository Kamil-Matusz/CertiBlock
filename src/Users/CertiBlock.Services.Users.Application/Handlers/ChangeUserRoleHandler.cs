using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;

namespace CertiBlock.Services.Users.Application.Handlers;

public sealed class ChangeUserRoleHandler(IUserRepository userRepository) : ICommandHandler<ChangeUserRole>
{
    public async Task HandlerAsync(ChangeUserRole command)
    {
        var role = new Role(command.Role);
        await userRepository.ChangeUserRoleAsync(command.UserId, role);
    }
}