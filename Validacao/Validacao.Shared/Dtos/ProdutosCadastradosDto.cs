using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
