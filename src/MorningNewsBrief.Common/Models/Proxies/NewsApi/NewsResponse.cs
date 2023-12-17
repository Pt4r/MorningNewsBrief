using Indice.Types;
using MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters;
using System.Text.Json.Serialization;

namespace MorningNewsBrief.Common.Models.Proxies.NewsApi {
    public class NewsResponse {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; } = null;

        [JsonPropertyName("message")]
        public string? Message { get; set; } = null;

        [JsonPropertyName("totalResults")]
        public int? TotalResults { get; set; } = null;

        [JsonPropertyName("articles")]
        public List<NewsArticleResponse>? Articles { get; set; } = null;


        public News ToModel(ListOptions<NewsFilter> options) =>
            new() {
                Articles = Articles?.Select(article =>
                                new NewsArticles {
                                    SourceName = article.Source.Name,
                                    Author = article.Author,
                                    Title = article.Title,
                                    Url = article.Url,
                                    PublishedAt = article.PublishedAt
                                }).AsQueryable().ToResultSet(options)
            };
    }

    public class NewsSourceResponse {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class NewsArticleResponse {
        [JsonPropertyName("source")]
        public NewsSourceResponse Source { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("urlToImage")]
        public object UrlToImage { get; set; }

        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("content")]
        public object Content { get; set; }
    }

}
