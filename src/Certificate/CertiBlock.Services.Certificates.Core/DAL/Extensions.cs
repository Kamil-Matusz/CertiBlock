using CertiBlock.Services.Certificates.Core.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Certificates.Core.DAL;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        
        return services;
    }
}