using CertiBlock.Services.Users.Infrastructure.Exceptions;

namespace CertiBlock.Services.Users.Infrastructure.Errors;

internal interface IExceptionCompositionRoot
{
    ExceptionResponse Map(Exception exception);
}