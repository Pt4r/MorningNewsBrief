using Indice.Types;
using MorningNewsBrief.Common.Models.Proxies.WeatherApi.Filters;

namespace MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters {
    public class NewsBriefingFilter {
        public ListOptions<NewsFilter> NewsListOptions { get; set; } = new ListOptions<NewsFilter>();
        public ListOptions<WeatherFilter> WeatherListOptions { get; set; } = new ListOptions<WeatherFilter>();
    }
}
