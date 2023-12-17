using Indice.Serialization;
using Indice.Types;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MorningNewsBrief.Common.Configuration;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using MorningNewsBrief.Common.Models.Proxies.SpotifyApi;
using MorningNewsBrief.Common.Models.Proxies.SpotifyApi.Filters;
using MorningNewsBrief.Common.Models.Proxies.WeatherApi;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MorningNewsBrief.Common.Services.Proxies {
    public class SpotifyProxy {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SpotifyProxy> _logger;
        private readonly GeneralSettings _settings;
        private readonly IDistributedCache _cache;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public string API_NAME = "Spotify";

        public SpotifyProxy(HttpClient httpClient, ILogger<SpotifyProxy> logger, GeneralSettings settings, IDistributedCache distributedCache) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _cache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(_settings.Endpoints[API_NAME].Address.TrimEnd('/') + "/");
            _clientId = _settings.Endpoints[API_NAME].ClientId;
            _clientSecret = _settings.Endpoints[API_NAME].ClientSecret;
        }

        public async Task<MusicRecommendations?> GetRecommendations(ListOptions<SpotifyFilter> options) {
            try {
                return await ProcessGetRecommendations(options);
            } catch (HttpRequestException ex) {
                _logger.LogError($"There was a problem while retrieving {API_NAME} information. Error with status '{ex.StatusCode}' is '{ex.Message}'.");
                throw;
            } catch (Exception ex) {
                _logger.LogError($"There was an problem while retrieving {API_NAME} information. Error is '{ex.Message}'.");
            }
            return default;
        }

        private async Task<MusicRecommendations> ProcessGetRecommendations(ListOptions<SpotifyFilter> options) {
            var accessToken = await GetAccessTokenAsync();

            // Build the query string
            var query = new StringBuilder();

            query.Append($"market={options.Filter.Market}");
            query.Append($"&seed_genres={string.Join(",", options.Filter.Genres)}");
            if (options.Size == ListOptions.DEFAULT_SIZE) {
                query.Append($"&limit=5");
            } else {
                query.Append($"&limit={options.Size!.Value}");
            }

            var uri = new Uri($"recommendations?{query}", UriKind.Relative);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpRequest.Headers.Add("User-Agent", "Morning News Brif App");

            var httpResponseMessage = await _httpClient.SendAsync(httpRequest);
            var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            if (!httpResponseMessage.IsSuccessStatusCode) {
                if (httpResponseMessage.StatusCode == HttpStatusCode.TooManyRequests) {
                    throw new HttpRequestException($"You made too many requests within a window of time and have been rate limited.", null, HttpStatusCode.TooManyRequests);
                } else {
                    _logger.LogCritical($"Cannot fetch music recommendations. Status code: {httpResponseMessage.StatusCode}");
                    throw new HttpRequestException($"{httpResponseMessage.ReasonPhrase}", null, HttpStatusCode.BadGateway);
                }
            }

            MusicRecommendationsResponse? response = null;
            try {
                response = JsonSerializer.Deserialize<MusicRecommendationsResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
            } catch (Exception) {
                _logger.LogCritical("Cannot deserialize music recommendations.");
            }
            return response == null
                ? throw new Exception($"No results could be fetched with the query {query}")
                : response.ToModel(options);
        }

        private async Task<string> GetAccessTokenAsync() {
            const string cacheKey = "spotify_access_token";
            var accessToken = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(accessToken)) {
                return accessToken;
            }
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")));
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var httpResponseMessage = await httpClient.SendAsync(request);
            var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            if (!httpResponseMessage.IsSuccessStatusCode) {
                if (httpResponseMessage.StatusCode == HttpStatusCode.TooManyRequests) {
                    throw new HttpRequestException($"You made too many requests within a window of time and have been rate limited.", null, HttpStatusCode.TooManyRequests);
                } else {
                    throw new HttpRequestException($"{httpResponseMessage.ReasonPhrase}", null, HttpStatusCode.BadGateway);
                }
            }

            SpotifyAuthenticationResponse? response = null;
            try {
                response = JsonSerializer.Deserialize<SpotifyAuthenticationResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
            } catch (Exception) {
                _logger.LogCritical("Cannot deserialize sptofy access token response.");
            }

            if (response == null) {
                throw new Exception($"The spotify access token could not be fetched.");
            }
            accessToken = response.Access_Token;
            var cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.SetAbsoluteExpiration(TimeSpan.FromSeconds(response.Expires_In - 300));
            await _cache.SetStringAsync(cacheKey, accessToken, cacheOptions);

            return accessToken;
        }
    }
}
