using System.Security.Claims;

namespace Validacao.Application.Interfaces
{
    public interface IAuthStateProvider
    {
        Task MarcarComoLogado(string sessionJson);
        Task MarcarComoDeslogado();
        // 💡 Adicione esta linha exata que estava faltando no contrato!
        Task<ClaimsPrincipal> ObterUsuarioAtualAsync();
    }
}