using CertiBlock.Services.Certificates.Core.DAL;
using CertiBlock.Services.Certificates.Core.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Certificates.Core;

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidator();
        services.AddMongo(configuration);
        return services;
    }
}