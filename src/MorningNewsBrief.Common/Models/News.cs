namespace MorningNewsBrief.Common.Models {
    public class News {
        public IList<NewsArticles>? Articles { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class NewsArticles {
        public string SourceName { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime PublishedAt { get; set; }

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
        Gr = 0,
        Us,
        It
    }

    public enum NewsLanguage {
        En = 0,
        Es,
        De,
        Fr
    }
}
