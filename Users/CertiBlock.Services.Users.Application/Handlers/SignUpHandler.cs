using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Application.Services.Clock;
using CertiBlock.Services.Users.Core.Entities;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Core.ValueObjects;

namespace CertiBlock.Services.Users.Application.Handlers;

internal sealed class SignUpHandler : ICommandHandler<SignUp>
{
    private readonly IClock _clock;
    private readonly IPasswordManager _passwordManager;
    private IUserRepository _userRepository;

    public SignUpHandler(IClock clock, IPasswordManager passwordManager, IUserRepository userRepository)
    {
        _clock = clock;
        _passwordManager = passwordManager;
        _userRepository = userRepository;
    }
    
    public async Task HandlerAsync(SignUp command)
    {
        var role = string.IsNullOrWhiteSpace(command.Role) ? Role.User() : new Role(command.Role);
        
        if (await _userRepository.GetUserByEmailAsync(command.Email) is not null)
        {
            throw new EmailAlreadyInUseException(command.Email);
        }
        
        var securedPassword = _passwordManager.Secure(command.Password);
        var user = new User(command.UserId, command.Email, securedPassword, command.Role, command.IsActive, _clock.CurrentDate());

        await _userRepository.AddUserAsync(user);
    }
}