using CertiBlock.Services.Users.Application.Abstractions;

namespace CertiBlock.Services.Users.Application.Commands;

public record ChangeUserPassword(Guid UserId, string CurrentPassword, string NewPassword) : ICommand;