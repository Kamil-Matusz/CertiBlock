using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Users.Core.Exceptions;

public class AccountIsNotActiveException(string email) : CustomException("The account is inactive. Unable to Sign In.")
{
    public string Email { get; } = email;
}