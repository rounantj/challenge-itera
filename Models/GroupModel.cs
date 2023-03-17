using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace IteraCompanyGroups.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public DateTime DateIngestion { get; set; }

        public DateTime LastUpdate { get; set; }
        public ICollection<Company> Companys { get; set; }
    }

    public class GroupRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("date_ingestion")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime DateIngestion { get; set; }

        [JsonPropertyName("last_update")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime LastUpdate { get; set; }

    }
}
