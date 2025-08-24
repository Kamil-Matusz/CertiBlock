using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}