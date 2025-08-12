namespace CertiBlock.Services.Users.Core.Exceptions;

public sealed class EmailAlreadyInUseException(string email) : CustomException($"Email: '{email}' is already in use.")
{
    public string Email { get; } = email;
}