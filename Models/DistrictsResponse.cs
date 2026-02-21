using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace to_integrations.Models
{
    public class DistrictsResponse
    {
        [JsonPropertyName("Code")]
        public string Code { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }

        [JsonPropertyName("Data")]
        public List<DistrictItem> Data { get; set; }
    }

    public class DistrictItem
    {
        [JsonPropertyName("districtid")]
        public string DistrictId { get; set; }

        [JsonPropertyName("districtname")]
        public string DistrictName { get; set; }

        [JsonPropertyName("cityid")]
        public string CityId { get; set; }
    }
}
