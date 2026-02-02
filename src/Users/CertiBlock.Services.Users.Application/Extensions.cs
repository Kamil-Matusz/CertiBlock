using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Services;
using CertiBlock.Services.Users.Application.Services.Clock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Users.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(ICommandHandler<>).Assembly;
        services.Scan(s => s.FromAssemblies(applicationAssembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    
        // Services
        services.AddServices();
        
        return services;
    }
}