using CertiBlock.Services.Users.Core.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Users.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddFluentValidator();
        return services;
    }
}