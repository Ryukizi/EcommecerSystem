namespace Validacao.Shared.Dtos
{
    public class UsuarioCadastroDto
    {
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
