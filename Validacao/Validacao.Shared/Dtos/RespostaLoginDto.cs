namespace Validacao.Shared.Dtos
{
    public class RespostaLogin
    {
        public string Message { get; set; } = string.Empty;
        public UsuarioInfo? Usuario { get; set; }
    }
}
