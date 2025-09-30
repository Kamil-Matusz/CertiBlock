using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Services.Ethereum.Core.Validators;

public static class Extensions
{
    public static IServiceCollection AddFluentValidator(this IServiceCollection services)
    {
        services.AddFluentValidation(fv => fv
            .RegisterValidatorsFromAssemblyContaining<EthereumAddressDtoValidator>()
            .RegisterValidatorsFromAssemblyContaining<MetricDtoValidator>()
            .RegisterValidatorsFromAssemblyContaining<BlockchainTransactionDtoValidator>());

        return services;
    }
}