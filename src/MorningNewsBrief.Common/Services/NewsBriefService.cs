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
            // Retieve api data
            var newsTask = _newsApi.GetNews(options.Filter.NewsListOptions);

            Task.WhenAll(newsTask);

            // Get the data or the cached version if there was an error.
            var news = await newsTask ?? await GetCached<News, NewsFilter>(options.Filter.NewsListOptions);

            // Aggregate the data
            var brief = new NewsBriefing() {
                News = news
            };

            // Set the cached versions
            await SetCached(news, options.Filter.NewsListOptions);

            return brief;
        }

        public async Task<T?> GetCached<T, TFilter>(ListOptions<TFilter>? options) where TFilter : class, new() {
            T? response = default;
            var cachedNews = await _cache.GetStringAsync($"{nameof(T)}_{options}");
            if (cachedNews != null) {
                response = JsonSerializer.Deserialize<T>(cachedNews, JsonSerializerOptionDefaults.GetDefaultSettings());
            }
            return response;
        }

        public async Task SetCached<T, TFilter>(T? news, ListOptions<TFilter>? options) where T : class where TFilter : class, new() {
            if (news != null) {
                var cacheOptions = new DistributedCacheEntryOptions();
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                var serializedNews = JsonSerializer.Serialize(news, JsonSerializerOptionDefaults.GetDefaultSettings());
                await _cache.SetStringAsync($"{nameof(T)}_{options}", serializedNews, cacheOptions);
            }
        }
    }
}
