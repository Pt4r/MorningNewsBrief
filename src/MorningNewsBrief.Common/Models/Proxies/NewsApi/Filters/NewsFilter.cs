using MorningNewsBrief.Common.Models.Proxies.WeatherApi.Filters;

namespace MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters {
    public class NewsFilter {
        public NewsCategories? Category { get; set; } = null;
        public NewsCountry? Country { get; set; } = NewsCountry.Greece;
    }

    public enum NewsCategories {
        Business = 0,
        Entertainment,
        General,
        Health,
        Science,
        Sports,
        Technology
    }

    public enum NewsCountry {
        Greece = 0,
        UnitedStates,
        Italy
    }

    public static class NewsFilterExtensions {
        private static readonly Dictionary<NewsCountry, string> NewsCountryMap = new() {
                                    {NewsCountry.Greece, "gr"},
                                    {NewsCountry.UnitedStates, "us"},
                                    {NewsCountry.Italy, "it"}
                                };

        public static string ToShortForm(this NewsCountry language) {
            return NewsCountryMap[language];
        }
    }
}
