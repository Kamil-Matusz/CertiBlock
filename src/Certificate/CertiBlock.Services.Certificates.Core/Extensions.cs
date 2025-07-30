using CertiBlock.Services.Certificates.Core.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Certificates.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddFluentValidator();
        return services;
    }
}