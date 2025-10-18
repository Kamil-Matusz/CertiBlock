using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Certificates.Core.Services;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICertificateService, CertificateService>();
        
        return services;
    }
}