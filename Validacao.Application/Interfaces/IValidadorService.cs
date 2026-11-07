using Validacao.Shared.Models;

namespace Validacao.Application.Interfaces
{
    public interface IValidadorService
    {
        public Task<List<string>> EntradaDeDados(Pessoa pessoa);

        public bool Idade(Pessoa pessoa);

        public string Nome(Pessoa pessoa);

        public Task<string> Email(Pessoa pessoa);

        public string Senha(Pessoa pessoa);
    }
}
