using CertiBlock.Services.Users.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Users.Infrastructure.Errors;

internal class ExceptionCompositionRoot : IExceptionCompositionRoot
{
    private readonly IServiceProvider _serviceProvider;

    public ExceptionCompositionRoot(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ExceptionResponse Map(Exception exception)
    {
        using var scope = _serviceProvider.CreateScope();
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