using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace to_integrations.Models
{
    public class CitiesResponse
    {
        [JsonPropertyName("Code")]
        public string Code { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }

        [JsonPropertyName("Data")]
        public List<CityItem> Data { get; set; }
    }

    public class CityItem
    {
        [JsonPropertyName("cityid")]
        public string CityId { get; set; }

        [JsonPropertyName("cityname")]
        public string CityName { get; set; }
    }
}
