using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ToIntegrations.Models
{
    public class HotelsResponse
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
        public List<HotelItem> Data { get; set; }
    }

    public class HotelItem
    {
        [JsonPropertyName("hotelid")]
        public string HotelId { get; set; }

        [JsonPropertyName("hotelname")]
        public string HotelName { get; set; }

        [JsonPropertyName("areaid")]
        public string AreaId { get; set; }

        [JsonPropertyName("areaname")]
        public string AreaName { get; set; }

        [JsonPropertyName("cityid")]
        public string CityId { get; set; }

        [JsonPropertyName("cityname")]
        public string CityName { get; set; }

        [JsonPropertyName("districtid")]
        public string DistrictId { get; set; }

        [JsonPropertyName("districtname")]
        public string DistrictName { get; set; }

        [JsonPropertyName("starrating")]
        public int? StarRating { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("latitude")]
        public string Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; }
    }
}
