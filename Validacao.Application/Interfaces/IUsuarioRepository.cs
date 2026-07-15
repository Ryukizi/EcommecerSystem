using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validacao.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        // Promete que quem implementar isto vai saber salvar um usuário
        Task<Usuario> InserirAsync(Usuario usuario);

        // Promete que quem implementar isto vai saber buscar um usuário pelo e-mail
        Task<Usuario?> ObterPorEmailAsync(string email);
    }
}
