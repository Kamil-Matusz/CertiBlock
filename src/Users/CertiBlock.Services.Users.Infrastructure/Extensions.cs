using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Infrastructure.Auth;
using CertiBlock.Services.Users.Infrastructure.DAL;
using CertiBlock.Services.Users.Infrastructure.DAL.Repositories;
using CertiBlock.Services.Users.Infrastructure.Security;
using CertiBlock.Shared.CORS;
using CertiBlock.Shared.Exceptions;
using CertiBlock.Shared.Logging;
using CertiBlock.Shared.Observability;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Users.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var infrastructureAssembly = typeof(AppOptions).Assembly;
        services.Scan(s => s.FromAssemblies(infrastructureAssembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        // PostgreSQL
        services.AddPostgres(configuration);
        
        // Repositories
        services.AddRepositories();
        
        services.AddErrorHandling();
        services.AddSingleton<IExceptionMapper, UserExceptionMapper>();
        
        services.AddControllers();

        // Logging
        services.AddSeqLogging(configuration);

        // Password Hasher
        services.AddSecurity();
        
        // JWT Token
        services.AddAuth(configuration);
        services.AddHttpContextAccessor();

        // HealthCheck
        services.AddHealthChecks();
        
        // CORS
        services.AddCorsPolicy();
        
        // OpenTelemetry
        services.AddObservability(configuration);
        
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseCorsPolicy();
        
        app.UseErrorHandling();
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
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