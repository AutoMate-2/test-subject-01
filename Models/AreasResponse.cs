using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ToIntegrations.Models
{
    public class AreasResponse
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
        public string ResponseID { get; set; }

        [JsonPropertyName("Data")]
        public List<AreaItem> Data { get; set; }
    }

    public class AreaItem
    {
        [JsonPropertyName("areaid")]
        public string AreaId { get; set; }

        [JsonPropertyName("areaname")]
        public string AreaName { get; set; }
    }
}
