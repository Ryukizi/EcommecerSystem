namespace Validacao.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> InserirAsync(Usuario usuario);

        Task<Usuario?> ObterPorEmailAsync(string email);

        Task<Usuario?> ObterPorRefreshTokenAsync(string refreshToken);
        Task AtualizarAsync(Usuario usuario);
    }
}
