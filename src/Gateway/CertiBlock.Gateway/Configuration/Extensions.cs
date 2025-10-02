namespace CertiBlock.Gateway.Configuration;

public static class Extensions
{
    public static IServiceCollection AddGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAuth", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("AllowAnonymous", policy => policy.RequireAssertion(_ => true));
        });

        
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        return services;  
    }
}