using CertiBlock.Gateway.Configuration;
using CertiBlock.Shared.CORS;
using CertiBlock.Shared.Logging;
using CertiBlock.Shared.Observability;

namespace CertiBlock.Gateway;

public static class Extensions
{
    public static IServiceCollection AddGatewayExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        // CORS
        services.AddCorsPolicy(configuration);

        // Logger
        services.AddLogging();

        // Seq
        services.AddSeqLogging(configuration);

        // Gateway
        services.AddGateway(configuration);

        // Observability
        services.AddObservability(configuration);

        return services;
    }
}