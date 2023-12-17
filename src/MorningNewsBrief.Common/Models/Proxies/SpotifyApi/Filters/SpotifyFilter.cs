namespace MorningNewsBrief.Common.Models.Proxies.SpotifyApi.Filters {
    public class SpotifyFilter {
        public string Market { get; set; } = "GR";
        public string[] Genres { get; set; } = new string[] { "pop", "rock", "jazz", "classical", "metal"};
    }
}
