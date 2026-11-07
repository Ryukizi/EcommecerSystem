namespace Validacao.Shared.Dtos
{
    public class UsuarioLoginDto
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
