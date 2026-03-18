using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace CertiBlock.Gateway.Configuration;

public static class Extensions
{
    private const string SectionName = "Auth";
    public const string AuthRateLimitPolicy = "auth-limit";

    public static IServiceCollection AddGateway(this IServiceCollection services, IConfiguration configuration)
    {
        var authSection = configuration.GetRequiredSection(SectionName);
        var issuer = authSection["Issuer"]!;
        var audience = authSection["Audience"]!;
        var signingKey = authSection["SigningKey"]!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAuth", policy => policy.RequireAuthenticatedUser());
        });

        services.AddRateLimiter(options =>
        {
            options.AddPolicy<IPAddress, AuthRateLimiterPolicy>(AuthRateLimitPolicy);
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        services
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        return services;
    }
}