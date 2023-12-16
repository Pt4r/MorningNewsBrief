using Swashbuckle.AspNetCore.Annotations;

namespace MorningNewsBrief.Common.Models
{
    /// <summary>
    /// News Briefing Model
    /// </summary> 
    public class NewsBriefing
    {
        [SwaggerSchema(
            Title = "List of News",
            Description = "This is object containg all the daily news.",
            Format = "News")]
        public News? News { get; set; }
    }
}
