namespace MorningNewsBrief.Common.Models.Proxies.NewsApi.Filters {
    public class NewsFilter {
        public NewsCategories? Category { get; set; } = null;
        public NewsLanguage? Language { get; set; } = null;
        public NewsCountry? Country { get; set; } = null;
    }
}
