using Indice.Serialization;
using Indice.Types;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using MorningNewsBrief.Common.Models.Proxies.SpotifyApi.Filters;
using MorningNewsBrief.Common.Models.Proxies.WeatherApi.Filters;
using MorningNewsBrief.Common.Services.Abstractions;
using MorningNewsBrief.Common.Services.Proxies;
using System.Text.Json;

namespace MorningNewsBrief.Common.Services {
    public class NewsBriefService : INewsBriefService {
        private readonly ILogger<NewsBriefService> _logger;
        private readonly NewsProxy _newsApi;
        private readonly WeatherProxy _weatherApi;
        private readonly SpotifyProxy _musicProxy;
        private readonly IDistributedCache _cache;

        public NewsBriefService(NewsProxy newsProxy, WeatherProxy weatherProxy,SpotifyProxy musicProxy, ILogger<NewsBriefService> logger, IDistributedCache distributedCache) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _newsApi = newsProxy ?? throw new ArgumentNullException(nameof(newsProxy));
            _weatherApi = weatherProxy ?? throw new ArgumentNullException(nameof(weatherProxy));
            _musicProxy = musicProxy ?? throw new ArgumentNullException(nameof(musicProxy));
            _cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public async Task<NewsBriefing> GetNewsBriefing(ListOptions<NewsBriefingFilter> options, CancellationToken cancellationToken) {
            // Get the cached versions
            var news = await GetCached<News, NewsFilter>(options.Filter.NewsListOptions, cancellationToken);
            var weather = await GetCached<Weather, WeatherFilter>(options.Filter.WeatherListOptions, cancellationToken);
            var music = await GetCached<MusicRecommendations, SpotifyFilter>(options.Filter.SpotifyListOptions, cancellationToken);

            if (news?.Articles != null && weather != null && music?.Tracks != null) {
                // Aggregate the data
                return new NewsBriefing() {
                    News = news,
                    Weather = weather,
                    MusicRecommendations = music
                };
            }

            // Get the actual data if the cached version is not found
            List<Task> tasks = new();
            var newsTask = news?.Articles == null ? Task.Run(() => _newsApi.GetNews(options.Filter.NewsListOptions), cancellationToken) : default;
            var weatherTask = weather == null ? Task.Run(() => _weatherApi.GetCurrentWeather(options.Filter.WeatherListOptions), cancellationToken) : default;
            var musicTask = music?.Tracks == null ? Task.Run(() => _musicProxy.GetRecommendations(options.Filter.SpotifyListOptions), cancellationToken) : default;
            if (newsTask != null) {
                tasks.Add(newsTask);
            }
            if (weatherTask != null) {
                tasks.Add(weatherTask);
            }
            if (musicTask != null) {
                tasks.Add(musicTask);
            }

            // Run all api calls in parallel
            await Task.WhenAll(tasks);

            // Get the data or the cached version if there was an error.
            news = newsTask != null 
                ? await newsTask 
                : await GetCached<News, NewsFilter>(options.Filter.NewsListOptions, cancellationToken);
            weather = weatherTask != null
                ? await weatherTask
                : await GetCached<Weather, WeatherFilter>(options.Filter.WeatherListOptions, cancellationToken);
            music = musicTask != null
                ? await musicTask
                : await GetCached<MusicRecommendations, SpotifyFilter>(options.Filter.SpotifyListOptions, cancellationToken);

            // Set the cached versions
            await SetCached(news, options.Filter.NewsListOptions, 5, cancellationToken);
            await SetCached(weather, options.Filter.WeatherListOptions, cancellationToken: cancellationToken);
            await SetCached(music, options.Filter.SpotifyListOptions, 60, cancellationToken);

            return new NewsBriefing() {
                News = news,
                Weather = weather,
                MusicRecommendations = music
            };
        }

        private async Task<T?> GetCached<T, TFilter>(ListOptions<TFilter>? options, CancellationToken cancellationToken = default) where TFilter : class, new() {
            T? response = default;
            var optionsString = string.Empty;
            if (options != null) {
                var optionsDict = options?.ToDictionary();
                optionsString = optionsDict != null ? string.Join("_", optionsDict.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value)) : string.Empty;
            }
            var cachedNews = await _cache.GetStringAsync($"{nameof(T)}_{optionsString}", cancellationToken);
            if (cachedNews != null) {
                response = JsonSerializer.Deserialize<T>(cachedNews, JsonSerializerOptionDefaults.GetDefaultSettings());
            }
            return response;
        }

        private async Task SetCached<T, TFilter>(T? news, ListOptions<TFilter>? options, double duration = 15, CancellationToken cancellationToken = default) where T : class where TFilter : class, new() {
            if (news != null) {
                var optionsString = string.Empty;
                if (options != null) {
                    var optionsDict = options?.ToDictionary();
                    optionsString = optionsDict != null ? string.Join("_", optionsDict.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value)) : string.Empty;
                }
                var cacheOptions = new DistributedCacheEntryOptions();
                // Cache for 15 minutes
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(duration));
                var serializedNews = JsonSerializer.Serialize(news, JsonSerializerOptionDefaults.GetDefaultSettings());
                await _cache.SetStringAsync($"{nameof(T)}_{optionsString}", serializedNews, cacheOptions, cancellationToken);
            }
        }
    }
}
