using CertiBlock.Services.Certificates.Core.Auth;
using CertiBlock.Services.Certificates.Core.DAL;
using CertiBlock.Services.Certificates.Core.MassTransit;
using CertiBlock.Services.Certificates.Core.Services;
using CertiBlock.Services.Certificates.Core.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Certificates.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidator();
        
        // MongoDB
        services.AddMongo(configuration);
        
        // MassTransit
        services.AddMassTransitWithRabbitMq();

        // Services
        services.AddScoped<ICertificateService, CertificateService>();
        
        services.AddControllers();
        
        // JWT Token
        services.AddAuth(configuration);
        
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