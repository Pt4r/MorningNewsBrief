using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DistributedCacheConfig
    {
        public static IServiceCollection AddDistributedCacheConfig(this IServiceCollection services, IConfiguration configuration) {
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(redisConnectionString)) {
                services.AddDistributedMemoryCache();
            } else {
                services.AddStackExchangeRedisCache(options => {
                    options.Configuration = redisConnectionString;
                });
            }
            return services;
        }
    }
}
