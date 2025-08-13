namespace CertiBlock.Services.Users.Core.Exceptions;

public class UserNotFoundException(Guid id) : CustomException($"User with ID: '{id}' was not found.")
{
    public Guid Id { get; set; } = id;
}