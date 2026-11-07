using Supabase;
using Validacao.Application.Interfaces.Repositories;


namespace Validacao.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly Client _supabaseClient;

        // A Infraestrutura recebe o cliente do Supabase aqui
        public UsuarioRepository(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Usuario> InserirAsync(Usuario usuario)
        {
            // Código que estava antes no teu Service, isolado aqui!
            var response = await _supabaseClient.From<Usuario>().Insert(usuario);
            return response.Models.First();
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            // Código de busca que estava no teu Service, isolado aqui!
            var response = await _supabaseClient.From<Usuario>().Where(u => u.Email == email).Get();
            return response.Models.FirstOrDefault();
        }
    }
}
