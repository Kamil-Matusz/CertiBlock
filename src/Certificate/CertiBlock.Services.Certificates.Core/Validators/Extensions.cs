using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Certificates.Core.Validators;

public static class Extensions
{
    public static IServiceCollection AddFluentValidator(this IServiceCollection services)
    {
        services.AddFluentValidation(fv => fv
            .RegisterValidatorsFromAssemblyContaining<BlockchainRegisterRequestValidator>()
            .RegisterValidatorsFromAssemblyContaining<CertificateRequestValidator>());

        return services;
    }
}