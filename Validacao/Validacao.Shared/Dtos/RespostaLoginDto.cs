namespace Validacao.Shared.Dtos
{
    public class RespostaLogin
    {
        [System.Text.Json.Serialization.JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("usuario")]
        public UsuarioInfo? Usuario { get; set; }
    }
}
