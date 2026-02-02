using CertiBlock.Services.Users.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Users.Infrastructure.DAL.Repositories;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}