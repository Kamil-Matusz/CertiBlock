using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Certificates.Core.DAL.Repositories;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        
        return services;
    } 
}