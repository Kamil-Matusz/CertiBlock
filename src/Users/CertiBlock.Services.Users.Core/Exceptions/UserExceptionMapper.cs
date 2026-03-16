using System.Net;
using CertiBlock.Shared.Exceptions;

namespace CertiBlock.Services.Users.Core.Exceptions;

public sealed class UserExceptionMapper : IExceptionMapper
{
    public ExceptionResponse? Map(Exception exception)
        => exception switch
        {
            UserNotFoundException ex =>
                new ExceptionResponse(new { code = "user_not_found", message = ex.Message }, HttpStatusCode.NotFound),

            EmailAlreadyInUseException ex =>
                new ExceptionResponse(new { code = "email_already_in_use", message = ex.Message }, HttpStatusCode.Conflict),

            AccountIsNotActiveException ex =>
                new ExceptionResponse(new { code = "account_not_active", message = ex.Message }, HttpStatusCode.Forbidden),

            InvalidRoleException ex =>
                new ExceptionResponse(new { code = "invalid_role", message = ex.Message }, HttpStatusCode.BadRequest),

            UserRoleNotExistException ex =>
                new ExceptionResponse(new { code = "user_role_not_exist", message = ex.Message }, HttpStatusCode.NotFound),

            _ => null
        };
}