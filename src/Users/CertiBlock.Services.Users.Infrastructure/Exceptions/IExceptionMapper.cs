namespace CertiBlock.Services.Users.Infrastructure.Exceptions;

public interface IExceptionMapper
{
    ExceptionResponse Map(Exception exception);
}