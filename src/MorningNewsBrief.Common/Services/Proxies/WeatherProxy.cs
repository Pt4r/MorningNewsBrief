using Indice.Serialization;
using Indice.Types;
using Microsoft.Extensions.Logging;
using MorningNewsBrief.Common.Configuration;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using MorningNewsBrief.Common.Models.Proxies.WeatherApi;
using MorningNewsBrief.Common.Models.Proxies.WeatherApi.Filters;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MorningNewsBrief.Common.Services.Proxies {
    public class WeatherProxy {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherProxy> _logger;
        private readonly GeneralSettings _settings;
        private readonly string _clientSecret;

        public string API_NAME = "Weather";

        public WeatherProxy(HttpClient httpClient, ILogger<WeatherProxy> logger, GeneralSettings settings) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(_settings.Endpoints[API_NAME].Address.TrimEnd('/') + "/");
            _clientSecret = _settings.Endpoints[API_NAME].ClientSecret;
        }

        public async Task<Weather?> GetCurrentWeather(ListOptions<WeatherFilter>? options = null) {
            try {
                return await ProcessGetCurrentWeather(options);
            } catch (HttpRequestException ex) {
                _logger.LogError($"There was an problem while retrieving {API_NAME} information. Error with status '{ex.StatusCode}' is '{ex.Message}'.");
            } catch (Exception ex) {
                _logger.LogError($"There was an problem while retrieving {API_NAME} information. Error is '{ex.Message}'.");
            }
            return default;
        }

        private async Task<Weather> ProcessGetCurrentWeather(ListOptions<WeatherFilter>? options = null) {
            // If the location is not Athens, Greece then we need to get the lat/lon coordinates
            if (options?.Filter.Location != "Athens, Greece") {
                var latLong = await GetLatLonByName(options.Filter.Location);
                options.Filter.Latitude = latLong.Lat.ToString();
                options.Filter.Longitude = latLong.Lon.ToString();
            }

            // Build the query string
            var query = new StringBuilder();
            query.Append($"&lang={options.Filter.Language.ToShortForm()}");
            query.Append($"&lat={options.Filter.Latitude}");
            query.Append($"&lon={options.Filter.Longitude}");
            query.Append($"&units=metric");

            var uri = new Uri($"data/2.5/weather?appid={_clientSecret}{query}", UriKind.Relative);
            var httpResponseContent = await GetAsync(uri);

            // Get the response or throw an exception
            WeatherResponse? response = null;
            try {
                response = JsonSerializer.Deserialize<WeatherResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
            } catch (Exception) {
                _logger.LogCritical("Cannot deserialize weather response.");
            }
            return response == null
                ? throw new Exception($"No results could be fetched with the query {query}")
                : response.ToModel();
        }

        private async Task<LatLong> GetLatLonByName(string locationName) {
            var uri = new Uri($"geo/1.0/direct?appid={_clientSecret}&limit=1&q={locationName}", UriKind.Relative);
            var httpResponseContent = await GetAsync(uri);

            WeatherLocationResponse? response = null;
            try {
                response = JsonSerializer.Deserialize<WeatherLocationResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
            } catch (Exception) {
                _logger.LogCritical("Cannot deserialize weather location response.");
            }
            return response == null
                ? throw new Exception($"No results could be fetched.")
                : new LatLong { Lat = response.Lat, Lon = response.Lon };
        }

        private async Task<string> GetAsync(Uri uri) {
            //TODO: Check api calls timeout and throw exception to get cached version
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            httpRequest.Headers.Add("User-Agent", "Morning News Brif App");
            var httpResponseMessage = await _httpClient.SendAsync(httpRequest);
            var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            if (!httpResponseMessage.IsSuccessStatusCode) {
                if (httpResponseMessage.StatusCode == HttpStatusCode.TooManyRequests) {
                    throw new HttpRequestException($"You made too many requests within a window of time and have been rate limited.", null, HttpStatusCode.TooManyRequests);
                } else {
                    throw new HttpRequestException($"{httpResponseMessage.ReasonPhrase}", null, HttpStatusCode.BadGateway);
                }
            }
            return httpResponseContent;
        }

        private class LatLong {
            public double Lat { get; set; }
            public double Lon { get; set; }
        }
    }
}
