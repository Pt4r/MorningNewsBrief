namespace MorningNewsBrief.Common.Models {
    public class Weather {
        public Weather() {
            LastUpdated = DateTimeOffset.UtcNow;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Location { get; set; }
        public string Temperature { get; set; }
        public string Wind { get; set; }
        public string Humidity { get; set; }
        public string Pressure { get; set; }
        public string Cloudiness { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
    }
}
