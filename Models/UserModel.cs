using System;
using System.Text.Json.Serialization;

namespace IteraCompanyGroups.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastLogin { get; set; }
    }
    public class UserRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("date_created")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("last_update")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime LastUpdate { get; set; }


        [JsonPropertyName("last_login")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime LastLogin { get; set; }
    }
}