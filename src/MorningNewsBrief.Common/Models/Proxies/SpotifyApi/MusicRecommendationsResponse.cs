using Indice.Types;
using MorningNewsBrief.Common.Models.Proxies.SpotifyApi.Filters;
using System.Text.Json.Serialization;

namespace MorningNewsBrief.Common.Models.Proxies.SpotifyApi {
    public class MusicRecommendationsResponse {
        [JsonPropertyName("tracks")]
        public List<Track> Tracks { get; set; }

        [JsonPropertyName("seeds")]
        public List<Seed> Seeds { get; set; }

        public class Track {
            [JsonPropertyName("album")]
            public Album Album { get; set; }

            [JsonPropertyName("artists")]
            public List<Artist> Artists { get; set; }

            [JsonPropertyName("disc_number")]
            public int DiscNumber { get; set; }

            [JsonPropertyName("duration_ms")]
            public int DurationMs { get; set; }

            [JsonPropertyName("explicit")]
            public bool Explicit { get; set; }

            [JsonPropertyName("external_ids")]
            public ExternalIds ExternalIds { get; set; }

            [JsonPropertyName("external_urls")]
            public ExternalUrls ExternalUrls { get; set; }

            [JsonPropertyName("href")]
            public string Href { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("is_local")]
            public bool IsLocal { get; set; }

            [JsonPropertyName("is_playable")]
            public bool IsPlayable { get; set; }

            [JsonPropertyName("linked_from")]
            public LinkedFrom LinkedFrom { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("popularity")]
            public int Popularity { get; set; }

            [JsonPropertyName("preview_url")]
            public string PreviewUrl { get; set; }

            [JsonPropertyName("track_number")]
            public int TrackNumber { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("uri")]
            public string Uri { get; set; }
        }

        public class Seed {
            [JsonPropertyName("initialPoolSize")]
            public int InitialPoolSize { get; set; }

            [JsonPropertyName("afterFilteringSize")]
            public int AfterFilteringSize { get; set; }

            [JsonPropertyName("afterRelinkingSize")]
            public int AfterRelinkingSize { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("href")]
            public object Href { get; set; }
        }


        public class LinkedFrom {
            [JsonPropertyName("external_urls")]
            public ExternalUrls ExternalUrls { get; set; }

            [JsonPropertyName("href")]
            public string Href { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("uri")]
            public string Uri { get; set; }
        }

        public class Album {
            [JsonPropertyName("album_type")]
            public string AlbumType { get; set; }

            [JsonPropertyName("artists")]
            public List<Artist> Artists { get; set; }

            [JsonPropertyName("external_urls")]
            public ExternalUrls ExternalUrls { get; set; }

            [JsonPropertyName("href")]
            public string Href { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("images")]
            public List<Image> Images { get; set; }

            [JsonPropertyName("is_playable")]
            public bool IsPlayable { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("release_date")]
            public string ReleaseDate { get; set; }

            [JsonPropertyName("release_date_precision")]
            public string ReleaseDatePrecision { get; set; }

            [JsonPropertyName("total_tracks")]
            public int TotalTracks { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("uri")]
            public string Uri { get; set; }
        }

        public class Artist {
            [JsonPropertyName("external_urls")]
            public ExternalUrls ExternalUrls { get; set; }

            [JsonPropertyName("href")]
            public string Href { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("uri")]
            public string Uri { get; set; }
        }

        public class ExternalIds {
            [JsonPropertyName("isrc")]
            public string Isrc { get; set; }
        }

        public class ExternalUrls {
            [JsonPropertyName("spotify")]
            public string Spotify { get; set; }
        }

        public class Image {
            [JsonPropertyName("height")]
            public int Height { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }

            [JsonPropertyName("width")]
            public int Width { get; set; }
        }

        public MusicRecommendations ToModel(ListOptions<SpotifyFilter> options) =>
            new() {
                Tracks = Tracks?.Select(article =>
                                new RecommendedTrack {
                                    Title = article.Name,
                                    AlbumName = article.Album.Name,
                                    Artist = article.Album.Artists.FirstOrDefault()?.Name ?? article.Artists.FirstOrDefault()?.Name,
                                    Url = article.ExternalUrls.Spotify
                                }).AsQueryable().ToResultSet(options)
            };
    }
}
