using System;
using System.Text.Json.Serialization;

namespace IteraEmpresaGrupos.Models
{
    public class Custo
    {
        public int Id { get; set; }
        public string Ano { get; set; }
        public string IdType { get; set; }
        public DateTime LastUpdate { get; set; }
        public float Value { get; set; }
        public int EmpresaId { get; set; }
    }
    public class CustoRequest
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

        [JsonPropertyName("Empresa_id")]
        public int EmpresaId { get; set; }
    }
}
