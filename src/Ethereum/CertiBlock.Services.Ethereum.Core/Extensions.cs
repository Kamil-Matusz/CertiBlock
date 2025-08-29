using CertiBlock.Services.Ethereum.Core.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddFluentValidator();
        return services;
    }
}