namespace Validacao.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        // Promete que quem implementar isto vai saber salvar um usuário
        Task<Usuario> InserirAsync(Usuario usuario);

        // Promete que quem implementar isto vai saber buscar um usuário pelo e-mail
        Task<Usuario?> ObterPorEmailAsync(string email);
    }
}
