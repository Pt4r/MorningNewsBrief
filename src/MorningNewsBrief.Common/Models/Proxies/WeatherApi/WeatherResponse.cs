using System.Text.Json.Serialization;

namespace MorningNewsBrief.Common.Models.Proxies.WeatherApi {
    public class WeatherResponse {
        [JsonPropertyName("coord")]
        public Coord Coord { get; set; }

        [JsonPropertyName("weather")]
        public List<WeatherModel> Weather { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("main")]
        public WeatherMain Main { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("wind")]
        public WeatherWind Wind { get; set; }

        [JsonPropertyName("clouds")]
        public WeatherClouds Clouds { get; set; }

        [JsonPropertyName("dt")]
        public int Dt { get; set; }

        [JsonPropertyName("sys")]
        public WeatherSys Sys { get; set; }

        [JsonPropertyName("timezone")]
        public int Timezone { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cod")]
        public int Cod { get; set; }

        public Weather ToModel() =>
            new() {
                Title = Weather[0].Main,
                Description = Weather[0].Description,
                Icon = Weather[0].Icon,
                Location = Name,
                Temperature = Main.Temp.ToString(),
                Wind = Wind.Speed.ToString(),
                Humidity = Main.Humidity.ToString(),
                Pressure = Main.Pressure.ToString(),
                Cloudiness = Clouds.All.ToString()
            };
    }

    public class WeatherClouds {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }

    public class Coord {
        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }
    }

    public class WeatherMain {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; }

        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; }

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }

    public class WeatherSys {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("sunrise")]
        public int Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public int Sunset { get; set; }
    }

    public class WeatherModel {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("main")]
        public string Main { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }
    }

    public class WeatherWind {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }
    }
}
