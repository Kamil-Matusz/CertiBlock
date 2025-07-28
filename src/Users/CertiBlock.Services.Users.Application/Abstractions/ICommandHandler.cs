namespace CertiBlock.Services.Users.Application.Abstractions;

public interface ICommandHandler<in TCommand> where TCommand: class, ICommand
{
    Task HandlerAsync(TCommand command);
}