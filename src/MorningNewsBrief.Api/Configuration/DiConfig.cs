using AspNetCoreRateLimit;

namespace Microsoft.Extensions.DependencyInjection {
    public static class DiConfig {
        public static IServiceCollection AddDiConfig(this IServiceCollection services, IConfiguration configuration) {

            #region IP Rate Limiting
            // Load IP rate limiting general configuration from appsettings.json.
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            // Load IP rules from appsettings.json.
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
            // Inject counter and rules stores.
            services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            #endregion

            return services;
        }
    }
}
