using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace HomeServer.API.Configuration.RateLimiting
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));

            services.AddSingleton<IConfigureOptions<RateLimiterOptions>, ConfigureRateLimiterOptions>();

            services.AddRateLimiter(_ => { });

            return services;
        }
    }
}