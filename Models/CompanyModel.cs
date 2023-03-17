using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;


namespace IteraCompanyGroups.Models
{
    public class Company
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? DateIngestion { get; set; }
        public DateTime? LastUpdate { get; set; }
        public ICollection<Cost> Costs { get; set; }
    }

    public class CompanyRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("group_id")]
        public int GroupId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("date_ingestion")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? DateIngestion { get; set; }

        [JsonPropertyName("last_update")]
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? LastUpdate { get; set; }
    }

    // conversor customizado para DateTime
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParse(reader.GetString(), out DateTime result))
            {
                return result;
            }
            return default;
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
    }


}
