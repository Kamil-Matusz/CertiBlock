using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Shared.Exceptions;

public static class Extensions
{
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
        => services
            .AddScoped<ErrorHandlerMiddleware>()
            .AddScoped<RequestTimeLoggingMiddleware>()
            .AddSingleton<IExceptionMapper, ExceptionMapper>()
            .AddSingleton<IExceptionCompositionRoot, ExceptionCompositionRoot>();

    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        => app
            .UseMiddleware<ErrorHandlerMiddleware>()
            .UseMiddleware<RequestTimeLoggingMiddleware>();
}
