using Validacao.Shared.Dtos;

namespace Validacao.Application.Interfaces
{
    public interface IUsuarioAppService
    {
        Task<Usuario>CadastrarAsync(UsuarioCadastroDto request);
        Task<object> LoginAsync(UsuarioLoginDto request);
        Task<UsuarioLoginDto> ObterPerfilAsync(UsuarioLoginDto request);
        Task<string?> ValidarEmailAsync(string email);
        Task<int> EnviarCodigoAsync(string email);
    }
}
