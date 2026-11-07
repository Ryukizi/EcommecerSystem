using Validacao.Shared.Dtos;

namespace Validacao.Application.Interfaces
{
    public interface IFreteService
    {
    Task<List<FreteDto?>> CalcularFreteAsync(string cep);
    }
}



