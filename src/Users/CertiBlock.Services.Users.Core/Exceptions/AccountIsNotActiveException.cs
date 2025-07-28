namespace CertiBlock.Services.Users.Core.Exceptions;

public class AccountIsNotActiveException : CustomException
{
    public string Email { get; }
    
    public AccountIsNotActiveException(string email) : base("The account is inactive. Unable to Sign In.")
    {
        Email = email;
    }
}