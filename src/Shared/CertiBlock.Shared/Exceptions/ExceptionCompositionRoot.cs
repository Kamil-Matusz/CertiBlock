using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Shared.Exceptions;

internal sealed class ExceptionCompositionRoot(IServiceProvider serviceProvider) : IExceptionCompositionRoot
{
    public ExceptionResponse Map(Exception exception)
    {
        using var scope = serviceProvider.CreateScope();
        var mappers = scope.ServiceProvider.GetServices<IExceptionMapper>().ToArray();
        var nonDefaultMappers = mappers.Where(x => x is not ExceptionMapper);
        var result = nonDefaultMappers
            .Select(x => x.Map(exception))
            .SingleOrDefault(x => x is not null);

        if (result is not null)
        {
            return result;
        }

        var defaultMapper = mappers.SingleOrDefault(x => x is ExceptionMapper);

        return defaultMapper?.Map(exception);
    }
}