using Indice.Serialization;
using Indice.Types;
using Microsoft.Extensions.Logging;
using MorningNewsBrief.Common.Configuration;
using MorningNewsBrief.Common.Models;
using MorningNewsBrief.Common.Models.Proxies.NewsApi;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MorningNewsBrief.Common.Services {
    public sealed class NewsProxy {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NewsBriefService> _logger;
        private readonly GeneralSettings _settings;

        public string API_NAME = "News";

        public NewsProxy(HttpClient httpClient, ILogger<NewsBriefService> logger, GeneralSettings settings) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri(_settings.Endpoints[API_NAME].Address.TrimEnd('/') + "/");
        }

        public async Task<News?> GetNews(ListOptions<NewsFilter> options) {
            try {
                return await ProcessGetNews(options);
            } catch (HttpRequestException ex) {
                _logger.LogError($"There was an problem while retrieving {API_NAME} information. Error with status '{ex.StatusCode}' is '{ex.Message}'.");
            } catch (Exception ex) {
                _logger.LogError($"There was an problem while retrieving {API_NAME} information. Error is '{ex.Message}'.");
            }
            return default;
        }

        private async Task<News> ProcessGetNews(ListOptions<NewsFilter> options) {
            // Build the query string
            var query = new StringBuilder();

            if (options.Filter.Country.HasValue) {
                // The default country will be Greece unless changed by the country property.
                query.Append($"country={options.Filter.Country.Value.ToShortForm()}");
            }
            if (options.Filter.Category.HasValue) {
                query.Append($"&category={options.Filter.Category.Value.ToString().ToLower()}");
            }
            if (options.Size.HasValue) {
                query.Append($"&pageSize={options.Size.Value}");
            }
            if (options.Page.HasValue) {
                query.Append($"&page={options.Page.Value}");
            }
            if (!string.IsNullOrEmpty(options.Search)) {
                query.Append($"&q={options.Search}");
            }

            var uri = new Uri($"top-headlines?{query}", UriKind.Relative);

            //TODO: Check api calls timeout and throw exception to get cached version
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Endpoints[API_NAME].ClientSecret);
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

            NewsResponse? response = null;
            try {
                response = JsonSerializer.Deserialize<NewsResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
            } catch (Exception) {
                _logger.LogCritical("Cannot deserialize news response.");
            }
            return response == null || response.Status != "ok" || response.TotalResults == 0
                ? throw new Exception($"No results could be fetched with the query {query}")
                : response.ToModel(options);
        }
    }
}
