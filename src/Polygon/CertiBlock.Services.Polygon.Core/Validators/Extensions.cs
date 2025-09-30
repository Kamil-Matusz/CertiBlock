using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Polygon.Core.Validators;

public static class Extensions
{
    public static IServiceCollection AddFluentValidator(this IServiceCollection services)
    {
        services.AddFluentValidation(fv => fv
            .RegisterValidatorsFromAssemblyContaining<PolygonAddressDtoValidator>()
            .RegisterValidatorsFromAssemblyContaining<MetricDtoValidator>()
            .RegisterValidatorsFromAssemblyContaining<BlockchainTransactionDtoValidator>());

        return services;
    }
}