using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace CertiBlock.Services.Users.Infrastructure.Auth;

public sealed class AuthRateLimiterPolicy : IRateLimiterPolicy<IPAddress>
{
    public const string PolicyName = "auth-limit";

    public RateLimitPartition<IPAddress> GetPartition(HttpContext httpContext)
    {
        var ip = httpContext.Connection.RemoteIpAddress ?? IPAddress.Loopback;

        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
        (context, _) =>
        {
            context.HttpContext.Response.Headers.RetryAfter = "60";
            return ValueTask.CompletedTask;
        };
}
