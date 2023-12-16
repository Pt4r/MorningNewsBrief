using Indice.Serialization;
using Indice.Types;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using MorningNewsBrief.Common.Services.Abstractions;
using System.Text.Json;

namespace MorningNewsBrief.Common.Services {
    public class NewsBriefService : INewsBriefFacade {
        private readonly ILogger<NewsBriefService> _logger;
        private readonly NewsProxy _newsApi;
        private readonly IDistributedCache _cache;

        public NewsBriefService(NewsProxy newsApi, ILogger<NewsBriefService> logger, IDistributedCache distributedCache) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _newsApi = newsApi ?? throw new ArgumentNullException(nameof(newsApi));
            _cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public async Task<NewsBriefing> GetNewsBriefing(ListOptions<NewsBriefingFilter> options) {
            var newsTask = _newsApi.GetNews(options.Filter.NewsListOptions);

            Task.WhenAll(newsTask);
            var news = await newsTask;

            if (news == null) {
                // If not found, try to get the cached version.
                var cachedNews = await _cache.GetAsync($"{NewsProxy.API_NAME}_{options}");
                if (cachedNews != null) {
                    news = JsonSerializer.Deserialize<News>(cachedNews, JsonSerializerOptionDefaults.GetDefaultSettings());
                }
            }
           
            _logger.LogInformation("loaded");
            var brief = new NewsBriefing() { 
                News = news
            };

            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            var serializedNews = JsonSerializer.Serialize(news, JsonSerializerOptionDefaults.GetDefaultSettings());
            await _cache.SetStringAsync($"{NewsProxy.API_NAME}_{options}", serializedNews, cacheOptions);

            return brief;
        }
    }
}
