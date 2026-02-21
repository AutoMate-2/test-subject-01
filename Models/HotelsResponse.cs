using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace to_integrations.Models
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
        public List<Hotel> Data { get; set; }
    }

    public class Hotel
    {
        [JsonPropertyName("hotelid")]
        public string Hotelid { get; set; }

        [JsonPropertyName("hotelname")]
        public string Hotelname { get; set; }

        [JsonPropertyName("pricestatus")]
        public string Pricestatus { get; set; }

        [JsonPropertyName("cityid")]
        public string Cityid { get; set; }

        [JsonPropertyName("cityname")]
        public string Cityname { get; set; }

        [JsonPropertyName("districtid")]
        public string Districtid { get; set; }

        [JsonPropertyName("districtname")]
        public string Districtname { get; set; }

        [JsonPropertyName("hasalcohol")]
        public bool Hasalcohol { get; set; }

        [JsonPropertyName("hasfreewifi")]
        public bool Hasfreewifi { get; set; }

        [JsonPropertyName("hasmall")]
        public bool Hasmall { get; set; }

        [JsonPropertyName("hasmetro")]
        public bool Hasmetro { get; set; }

        [JsonPropertyName("haspool")]
        public bool Haspool { get; set; }

        [JsonPropertyName("hotelclass")]
        public string Hotelclass { get; set; }

        [JsonPropertyName("hoteltype")]
        public string Hoteltype { get; set; }

        [JsonPropertyName("popular")]
        public bool Popular { get; set; }

        [JsonPropertyName("recommended")]
        public bool Recommended { get; set; }

        [JsonPropertyName("priceupdated")]
        public string Priceupdated { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("imageurl")]
        public string Imageurl { get; set; }

        [JsonPropertyName("areaid")]
        public string Areaid { get; set; }

        [JsonPropertyName("areaname")]
        public string Areaname { get; set; }

        [JsonPropertyName("countryid")]
        public string Countryid { get; set; }

        [JsonPropertyName("countryname")]
        public string Countryname { get; set; }

        [JsonPropertyName("countrycodeiso2")]
        public string Countrycodeiso2 { get; set; }

        [JsonPropertyName("countrycodeiso3")]
        public string Countrycodeiso3 { get; set; }
    }
}
