using MorningNewsBrief.Common.Models.Proxies.WeatherApi.Filters;

namespace MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters {
    public class NewsFilter {
        public NewsCategories? Category { get; set; } = null;
        public NewsLanguage? Language { get; set; } = null;
        public NewsCountry? Country { get; set; } = null;
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

    public enum NewsLanguage {
        English = 0,
        Spanish,
        German,
        French
    }

    public static class NewsFilterExtensions {
        private static readonly Dictionary<NewsCountry, string> NewsCountryMap = new() {
                                    {NewsCountry.Greece, "gr"},
                                    {NewsCountry.UnitedStates, "us"},
                                    {NewsCountry.Italy, "it"}
                                };
        private static readonly Dictionary<NewsLanguage, string> NewsLanguageMap = new() {
                                    {NewsLanguage.English, "en"},
                                    {NewsLanguage.Spanish, "es"},
                                    {NewsLanguage.German, "de"},
                                    {NewsLanguage.French, "fr"}
                                };

        public static string ToShortForm(this NewsCountry language) {
            return NewsCountryMap[language];
        }
        public static string ToShortForm(this NewsLanguage language) {
            return NewsLanguageMap[language];
        }
    }
}
