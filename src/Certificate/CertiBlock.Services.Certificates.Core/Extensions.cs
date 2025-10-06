using CertiBlock.Services.Certificates.Core.Auth;
using CertiBlock.Services.Certificates.Core.Clients;
using CertiBlock.Services.Certificates.Core.Configurations;
using CertiBlock.Services.Certificates.Core.DAL;
using CertiBlock.Services.Certificates.Core.DAL.Repositories;
using CertiBlock.Services.Certificates.Core.Entities;
using CertiBlock.Services.Certificates.Core.Services;
using CertiBlock.Services.Certificates.Core.Validators;
using CertiBlock.Shared.Logging;
using CertiBlock.Shared.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace CertiBlock.Services.Certificates.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidator();
        
        // MongoDB
        services.AddMongo(configuration);
        ConfigureMongoDbMappings();
        
        // HttpClients
        services.AddClients();

        // Repositories
        services.AddRepositories();

        // Services
        services.AddScoped<ICertificateService, CertificateService>();
        
        services.AddControllers();

        // HealthCheck
        services.AddHealthChecks();
        
        // JWT Token
        services.AddAuth(configuration);
        
        // Logger
        services.AddLogging();
        
        // Seq
        services.AddSeqLogging(configuration);
        
        return services;
    }
    
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
    
    private static void ConfigureMongoDbMappings()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Certificate)))
        {
            CertificateConfiguration.Configure();
        }
    }
}