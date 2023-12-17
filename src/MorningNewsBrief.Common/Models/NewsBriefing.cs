using Swashbuckle.AspNetCore.Annotations;

namespace MorningNewsBrief.Common.Models
{
    /// <summary>
    /// News Briefing Model
    /// </summary> 
    public class NewsBriefing {

        [SwaggerSchema(
            Title = "The current weather",
            Description = "This is object containg the current weather.",
            Format = "Weather")]
        public Weather? Weather { get; set; }

        [SwaggerSchema(
            Title = "List of News",
            Description = "This is object containg all the daily news.",
            Format = "News")]
        public News? News { get; set; }

        [SwaggerSchema(
            Title = "Music Recommendations",
            Description = "This is object containg all the music recommendations.",
            Format = "MusicRecommendations")]
        public MusicRecommendations? MusicRecommendations { get; set; }
    }
}
