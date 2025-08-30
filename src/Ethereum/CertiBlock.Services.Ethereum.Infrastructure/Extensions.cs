using CertiBlock.Services.Ethereum.Core.Ethereum;
using CertiBlock.Services.Ethereum.Infrastructure.DAL;
using CertiBlock.Shared.Logging;
using CertiBlock.Shared.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Logger
        services.AddLogging();
        
        // Seq
        services.AddSeqLogging(configuration);

        // MongoDB
        services.AddMongo(configuration);

        // Repositories
        services.AddRepositories();
        
        services.AddControllers();
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseRouting();
        
        return app;
    }
    
    public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
    {
        var options = new T();
        var section = configuration.GetRequiredSection(sectionName);
        section.Bind(options);

        return options;
    }
}