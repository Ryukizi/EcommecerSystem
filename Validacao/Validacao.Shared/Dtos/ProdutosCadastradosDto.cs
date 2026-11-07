using System.Text.Json.Serialization;

namespace Validacao.Shared.Dtos
{
    public class ProdutosCadastradosDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("preco")]
        public int Preco { get; set; }

        [JsonPropertyName("marca")]
        public string Marca { get; set; } = string.Empty;

        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }

        [JsonPropertyName("url_imagem")]
        public string Url_imagem { get; set; } = string.Empty;
    }
}
