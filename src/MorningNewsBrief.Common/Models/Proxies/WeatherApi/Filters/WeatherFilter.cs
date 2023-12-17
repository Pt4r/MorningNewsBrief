namespace MorningNewsBrief.Common.Models.Proxies.WeatherApi.Filters {
    public class WeatherFilter {
        public WeatherLanguage Language { get; set; }
        public string Location { get; set; } = "Athens, Greece";
        public string Latitude { get; set; } = "37.983810";
        public string Longitude { get; set; } = "23.727539";
    }

    public enum WeatherLanguage {
        Greek = 0,
        English,
        French,
        German
    }

    public static class WeatherFilterExtensions {
        private static readonly Dictionary<WeatherLanguage, string> LanguageMap = new Dictionary<WeatherLanguage, string> {
                                    {WeatherLanguage.Greek, "el"},
                                    {WeatherLanguage.English, "en"},
                                    {WeatherLanguage.French, "fr"},
                                    {WeatherLanguage.German, "de"}
                                };

        public static string ToShortForm(this WeatherLanguage language) {
            return LanguageMap[language];
        }
    }
}
