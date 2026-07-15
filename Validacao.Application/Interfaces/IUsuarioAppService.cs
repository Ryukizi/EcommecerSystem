using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validacao.Shared.Dtos;

namespace Validacao.Application.Interfaces
{
    public interface IUsuarioAppService
    {
        Task<Usuario>CadastrarAsync(UsuarioCadastroDto request);
        Task<object> LoginAsync(UsuarioLoginDto request);
        Task<string?> ValidarEmailAsync(string email);
        Task<int> EnviarCodigoAsync(string email);
    }
}
