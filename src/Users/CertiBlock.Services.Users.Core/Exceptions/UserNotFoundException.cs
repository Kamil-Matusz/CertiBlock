namespace CertiBlock.Services.Users.Core.Exceptions;

public class UserNotFoundException : CustomException
{
    public Guid Id { get; set; }
    
    public UserNotFoundException(Guid id) : base($"User with ID: '{id}' was not found.")
    {
        Id = id;
    }
}