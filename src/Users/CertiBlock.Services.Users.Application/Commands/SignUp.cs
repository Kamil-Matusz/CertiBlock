using CertiBlock.Services.Users.Application.Abstractions;

namespace CertiBlock.Services.Users.Application.Commands;

public record SignUp(Guid UserId, string Email, string Password, string Role, bool IsActive) : ICommand;