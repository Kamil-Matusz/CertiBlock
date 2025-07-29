namespace CertiBlock.Services.Users.Core.Exceptions;

public class UserRoleNotExistException : CustomException
{
    public UserRoleNotExistException() : base("This role don't exist")
    {
    }
}