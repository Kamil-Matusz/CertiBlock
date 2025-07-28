using CertiBlock.Services.Users.Application.Abstractions;

namespace CertiBlock.Services.Users.Application.Commands;

public record SignIn(string Email, string Password) : ICommand;