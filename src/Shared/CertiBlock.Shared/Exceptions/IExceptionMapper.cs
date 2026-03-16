namespace CertiBlock.Shared.Exceptions;

public interface IExceptionMapper
{
    ExceptionResponse? Map(Exception exception);
}