using System;
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

        [JsonPropertyName("Timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("Version")]
        public string Version { get; set; }

        [JsonPropertyName("ResponseID")]
        public string ResponseId { get; set; }

        [JsonPropertyName("Data")]
        public List<CityData> Data { get; set; }
    }

    public class CityData
    {
        [JsonPropertyName("cityid")]
        public string CityId { get; set; }

        [JsonPropertyName("cityname")]
        public string CityName { get; set; }

        [JsonPropertyName("areaid")]
        public string AreaId { get; set; }
    }
}
