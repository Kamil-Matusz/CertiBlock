using CertiBlock.Services.Users.Core.Repositories;
using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;
using CertiBlock.Services.Users.Infrastructure.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Users.Infrastructure.DAL;

public static class Extensions
{
    private const string SectionName = "postgres";

    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        services.Configure<PostgresOptions>(section);
        var options = configuration.GetOptions<PostgresOptions>(SectionName);
        
        services.AddDbContext<UsersDbContext>(x => x.UseNpgsql(options.ConnectionString));
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddHostedService<DatabaseInitializer>();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return services;
    }
    
    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetSection(sectionName);
        section.Bind(options);

        return options;
    }
}