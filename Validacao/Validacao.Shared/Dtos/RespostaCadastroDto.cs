namespace Validacao.Shared.Dtos
{
    public class UsuarioInfo
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("perfil")]
        public string Perfil { get; set; } = string.Empty;
    }
}
