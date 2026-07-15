using System.Security.Claims;
using System.Threading.Tasks;
using Validacao.Shared.Dtos;

namespace Validacao.Application.Interfaces
{
    public interface IAuthService
    {
        // Ações de Negócio puras (O que a tela dispara)
        Task<bool> FazerLogin(UsuarioLoginDto loginDto);
        Task FazerLogout();
        Task<int> SolicitarCodigoEnvioAsync(string email);
        Task CadastrarUsuarioCompletoAsync(UsuarioCadastroDto usuarioDto);
        Task<ClaimsPrincipal> ObterUsuarioAtual();
    }
}