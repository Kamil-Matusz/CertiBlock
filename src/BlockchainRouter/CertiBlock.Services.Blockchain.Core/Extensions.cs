using CertiBlock.Services.Blockchain.Core.Clients;
using CertiBlock.Services.Blockchain.Core.MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Blockchain.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        // MassTransit
        services.AddMassTransitWithRabbitMq();
        
        // HttpClients
        services.AddClients(configuration);
        
        // Logger
        services.AddLogging();
        
        return services;
    }
    
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}