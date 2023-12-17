using Indice.Serialization;
using Indice.Types;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using MorningNewsBrief.Common.Models.Proxies.WeatherApi.Filters;
using MorningNewsBrief.Common.Services.Abstractions;
using MorningNewsBrief.Common.Services.Proxies;
using System.Text.Json;

namespace MorningNewsBrief.Common.Services {
    public class NewsBriefService : INewsBriefFacade {
        private readonly ILogger<NewsBriefService> _logger;
        private readonly NewsProxy _newsApi;
        private readonly WeatherProxy _weatherApi;
        private readonly IDistributedCache _cache;

        public NewsBriefService(NewsProxy newsProxy, WeatherProxy weatherProxy, ILogger<NewsBriefService> logger, IDistributedCache distributedCache) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _newsApi = newsProxy ?? throw new ArgumentNullException(nameof(newsProxy));
            _weatherApi = weatherProxy ?? throw new ArgumentNullException(nameof(weatherProxy));
            _cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public async Task<NewsBriefing> GetNewsBriefing(ListOptions<NewsBriefingFilter> options) {
            // Retieve api data
            var newsTask = _newsApi.GetNews(options.Filter.NewsListOptions);
            var weatherTask = _weatherApi.GetCurrentWeather(options.Filter.WeatherListOptions);

            Task.WhenAll(newsTask, weatherTask);

            // Get the data or the cached version if there was an error.
            var news = await newsTask ?? await GetCached<News, NewsFilter>(options.Filter.NewsListOptions);
            var weather = await weatherTask ?? await GetCached<Weather, WeatherFilter>(options.Filter.WeatherListOptions);

            // Aggregate the data
            var brief = new NewsBriefing() {
                News = news,
                Weather = weather
            };

            // Set the cached versions
            await SetCached(news, options.Filter.NewsListOptions);
            await SetCached(weather, options.Filter.WeatherListOptions);

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
