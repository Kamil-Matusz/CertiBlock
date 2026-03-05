using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CertiBlock.Shared.CORS;

public static class Extensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection("Cors").Get<CorsOptions>() ?? new CorsOptions();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                if (corsOptions.AllowedOrigins.Length > 0)
                    builder.WithOrigins(corsOptions.AllowedOrigins);
                else
                    builder.AllowAnyOrigin();

                if (corsOptions.AllowedMethods.Length > 0)
                    builder.WithMethods(corsOptions.AllowedMethods);
                else
                    builder.AllowAnyMethod();

                if (corsOptions.AllowedHeaders.Length > 0)
                    builder.WithHeaders(corsOptions.AllowedHeaders);
                else
                    builder.AllowAnyHeader();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors();
    }
}