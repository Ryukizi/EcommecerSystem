using System.Text.Json.Serialization;

namespace Validacao.Shared.Dtos
{
    public class FreteDto
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("Uf")]
        public string Uf { get; set; } = string.Empty;

        [JsonPropertyName("ValorPac")]
        public decimal ValorPac { get; set; }

        [JsonPropertyName("PrazoPac")]
        public string PrazoPac { get; set; } = string.Empty;

        [JsonPropertyName("ValorSedex")]
        public decimal ValorSedex { get; set; }

        [JsonPropertyName("PrazoSedex")]
        public string PrazoSedex { get; set; } = string.Empty;
    }
}
