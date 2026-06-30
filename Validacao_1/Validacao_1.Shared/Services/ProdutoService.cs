using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Supabase;
using System.Text.Json;

namespace Validacao_1.Shared.Services
{
    public class ProdutoService
    {
        private readonly Supabase.Client _supabaseClient;

        // O construtor recebe o cliente do Supabase igualzinho ao outro serviço
        public ProdutoService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Guitarras> GetProdutoByIdAsync(long id)
        {
            try
            {
                // O .Single() garante que o Supabase devolva apenas um objeto do tipo "Guitarras"
                var response = await _supabaseClient
                    .From<Guitarras>()
                    .Where(x => x.Id == id)
                    .Single();

                return response; // Aqui retorna um objeto "Guitarras", combinando com o Task<Guitarras> do topo
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao buscar a guitarra com ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<Guitarras>> GetProdutosAsync()
        {
            try
            {
                var response = await _supabaseClient.From<Guitarras>().Get();
                return response.Models;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao buscar produtos: {ex.Message}", ex);
            }
        }
    }
}