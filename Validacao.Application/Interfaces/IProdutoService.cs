using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validacao.Shared.Dtos;

namespace Validacao.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<List<ProdutosCadastradosDto>> GetProdutosAsync();
        Task<ProdutosCadastradosDto> GetProdutoByIdAsync(long id);
        Task<List<ProdutosCadastradosDto>> BuscarGuitarrasAsync(string termoBusca);
    }
}
