using Indice.Types;

namespace MorningNewsBrief.Common.Models {
    public class MusicRecommendations {
        public MusicRecommendations()
        {
            LastUpdated = DateTimeOffset.UtcNow;
        }

        public ResultSet<RecommendedTrack>? Tracks { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
    }

    public class RecommendedTrack {
        public string AlbumName { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Url { get; set; }
    }
}
