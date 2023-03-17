using System;
using System.Text.Json.Serialization;

namespace IteraCompanyGroups.Models
{
    public class Cost
    {
        public int Id { get; set; }
        public string Ano { get; set; }
        public string IdType { get; set; }
        public DateTime LastUpdate { get; set; }
        public float Value { get; set; }
        public int CompanyId { get; set; }
    }
    public class CostRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("ano")]
        public string Ano { get; set; }

        [JsonPropertyName("id_type")]
        public string? IdType { get; set; }


        [JsonPropertyName("last_update")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? LastUpdate { get; set; }

        [JsonPropertyName("value")]
        public float Value { get; set; }

        [JsonPropertyName("company_id")]
        public int CompanyId { get; set; }
    }
}
