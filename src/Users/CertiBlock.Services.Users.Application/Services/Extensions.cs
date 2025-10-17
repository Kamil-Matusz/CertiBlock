using CertiBlock.Services.Users.Application.Services.Clock;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Users.Application.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IClock, Clock.Clock>();
        
        return services;
    }
}