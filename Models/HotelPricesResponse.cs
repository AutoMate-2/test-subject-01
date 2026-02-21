using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace to_integrations.Models
{
    public class HotelPricesResponse
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
        public List<HotelPrice> Data { get; set; }
    }

    public class HotelPrice
    {
        [JsonPropertyName("hotelid")]
        public string Hotelid { get; set; }

        [JsonPropertyName("roomid")]
        public string Roomid { get; set; }

        [JsonPropertyName("roomname")]
        public string Roomname { get; set; }

        [JsonPropertyName("boardid")]
        public string Boardid { get; set; }

        [JsonPropertyName("boardname")]
        public string Boardname { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("checkin")]
        public string Checkin { get; set; }

        [JsonPropertyName("checkout")]
        public string Checkout { get; set; }

        [JsonPropertyName("nights")]
        public int? Nights { get; set; }

        [JsonPropertyName("adults")]
        public int? Adults { get; set; }

        [JsonPropertyName("children")]
        public int? Children { get; set; }
    }
}
