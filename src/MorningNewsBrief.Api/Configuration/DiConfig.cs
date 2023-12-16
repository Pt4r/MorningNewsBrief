using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Services;
using MorningNewsBrief.Common.Services.Abstractions;

namespace Microsoft.Extensions.DependencyInjection {
    public static class DiConfig {
        public static IServiceCollection AddDiConfig(this IServiceCollection services, IConfiguration configuration) {
            #region Settings configuration
            services.Configure<GeneralSettings>(configuration.GetSection(GeneralSettings.Name));
            services.TryAddTransient(serviceProvider => serviceProvider.GetRequiredService<IOptions<GeneralSettings>>().Value);
            #endregion

            services.AddTransient<INewsBriefFacade, NewsBriefService>();
            #region Api Services
            services.AddHttpClient<NewsProxy>();
            #endregion

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
