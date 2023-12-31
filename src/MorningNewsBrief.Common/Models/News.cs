﻿using Indice.Types;

namespace MorningNewsBrief.Common.Models {
    public class News {
        public News()
        {
            LastUpdated = DateTimeOffset.UtcNow;
        }

        public ResultSet<NewsArticle>? Articles { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
    }

    public class NewsArticle {
        public string SourceName { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTimeOffset PublishedAt { get; set; }

    }
}
