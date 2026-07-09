using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace HomeServer.API.Configuration.RateLimiting
{
    internal sealed class ConfigureRateLimiterOptions
    : IConfigureOptions<RateLimiterOptions>
    {
        private readonly RateLimitingOptions _options;

        public ConfigureRateLimiterOptions(IOptions<RateLimitingOptions> options)
        {
            _options = options.Value;
        }

        public void Configure(RateLimiterOptions options)
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/problem+json";

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    type = "https://httpstatuses.com/429",
                    title = "Too Many Requests",
                    status = 429,
                    detail = "Rate limit exceeded. Try again later."
                }, cancellationToken);
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetSlidingWindowLimiter(GetUserKey(context), _ => CreateLimiter(_options.Global)));

            foreach (var policy in _options.Policies)
            {
                options.AddPolicy(policy.Key, 
                    context => RateLimitPartition.GetSlidingWindowLimiter(
                        GetPartitionKey(policy.Value.KeyType, context), 
                        _ => CreateLimiter(policy.Value)));
            }
        }

        private static SlidingWindowRateLimiterOptions CreateLimiter(RateLimitPolicyOptions policy)
        {
            return new SlidingWindowRateLimiterOptions
            {
                PermitLimit = policy.PermitLimit,
                Window = TimeSpan.FromSeconds(policy.WindowSeconds),
                SegmentsPerWindow = policy.SegmentsPerWindow,
                QueueLimit = 0,
                AutoReplenishment = true
            };
        }

        private static string GetPartitionKey(RateLimitKeyType keyType, HttpContext context)
        {
            return keyType switch
            {
                RateLimitKeyType.Ip => GetIp(context),
                RateLimitKeyType.User => GetUserKey(context),
                _ => GetIp(context)
            };
        }

        private static string GetUserKey(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                return context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? GetIp(context);
            }

            return GetIp(context);
        }

        private static string GetIp(HttpContext context) => context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
    }
}
