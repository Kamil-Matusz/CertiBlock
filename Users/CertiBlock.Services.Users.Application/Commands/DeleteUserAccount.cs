using CertiBlock.Services.Users.Application.Abstractions;

namespace CertiBlock.Services.Users.Application.Commands;

public record DeleteUserAccount(Guid UserId) : ICommand;