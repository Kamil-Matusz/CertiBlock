using CertiBlock.Services.Users.Application.Abstractions;

namespace CertiBlock.Services.Users.Application.Commands;

public record ChangeUserRole(Guid UserId, string Role) : ICommand;