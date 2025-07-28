using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.Repositories;

namespace CertiBlock.Services.Users.Application.Handlers;

internal sealed class ChangeUserPasswordHandler : ICommandHandler<ChangeUserPassword>
{
    private readonly IPasswordManager _passwordManager;
    private readonly IUserRepository _userRepository;

    public ChangeUserPasswordHandler(IPasswordManager passwordManager, IUserRepository userRepository)
    {
        _passwordManager = passwordManager;
        _userRepository = userRepository;
    }

    public async Task HandlerAsync(ChangeUserPassword command)
    {
        var securedPassword = _passwordManager.Secure(command.Password);

        await _userRepository.ChangeUserPassword(command.UserId, securedPassword);
    }
}