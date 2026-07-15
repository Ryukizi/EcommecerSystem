using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Validacao.Application.Interfaces;
using Validacao.Shared.Dtos;

namespace Validacao.Infrastructure.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly Supabase.Client? _supabaseClient;
        private readonly HttpClient _httpClient;

        // 💡 CONSTRUTOR INTELIGENTE: 
        // No Blazor, o SupabaseClient será injetado como nulo, então ele usará o HttpClient.
        // Na API, o SupabaseClient existirá, então ele usará a busca direta do banco!
        public ProdutoService(HttpClient httpClient, Supabase.Client? supabaseClient = null)
        {
            _httpClient = httpClient;
            _supabaseClient = supabaseClient;
        }

        public async Task<List<ProdutosCadastradosDto>> GetProdutosAsync()
        {
            try
            {
                // Se NÃO temos o cliente do Supabase, significa que estamos no Blazor (Web).
                // Buscamos via HTTP chamando o endpoint da API!
                if (_supabaseClient == null)
                {
                    return await _httpClient.GetFromJsonAsync<List<ProdutosCadastradosDto>>("api/produtos") ?? new();
                }

                // Se TEMOS o cliente (estamos na API), executamos o seu código original intacto:
                var response = await _supabaseClient.From<Guitarras>().Get();
                return response.Models.Select(g => MapearParaDto(g)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao buscar produtos: {ex.Message}", ex);
            }
        }

        public async Task<ProdutosCadastradosDto> GetProdutoByIdAsync(long id)
        {
            try
            {
                // Se estamos no Blazor (Web), busca via HTTP chamando a API
                if (_supabaseClient == null)
                {
                    return await _httpClient.GetFromJsonAsync<ProdutosCadastradosDto>($"api/produtos/{id}")
                           ?? throw new Exception($"Produto com ID {id} não encontrado na API.");
                }

                // Se estamos na API, mantém seu código original intacto:
                var response = await _supabaseClient
                    .From<Guitarras>()
                    .Where(x => x.Id == id)
                    .Single();

                if (response == null) return null;

                return MapearParaDto(response);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocorreu um erro ao buscar a guitarra com ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<ProdutosCadastradosDto>> BuscarGuitarrasAsync(string termoBusca)
        {
            try
            {
                // Se estamos no Blazor (Web), busca via HTTP chamando a API
                if (_supabaseClient == null)
                {
                    return await _httpClient.GetFromJsonAsync<List<ProdutosCadastradosDto>>($"api/produtos/buscar?termo={termoBusca}") ?? new();
                }

                // Se estamos na API, mantém seu código original intacto:
                Console.WriteLine($"\n[API] 🔍 Iniciando busca no Supabase para o termo: '{termoBusca}'");

                if (string.IsNullOrWhiteSpace(termoBusca))
                {
                    Console.WriteLine("[API] ⚠️ Termo de busca vazio. Retornando todos os produtos.");
                    return await GetProdutosAsync();
                }

                var response = await _supabaseClient
                    .From<Guitarras>()
                    .Where(g => g.Nome.Contains(termoBusca) || g.Marca.Contains(termoBusca))
                    .Get();

                var listaMapeada = response.Models.Select(g => MapearParaDto(g)).ToList();

                Console.WriteLine($"[API] ✅ Busca concluída! O Supabase retornou {response.Models.Count} registros cruus.");
                return listaMapeada;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] ❌ ERRO CRÍTICO na busca do Supabase: {ex.Message}");
                throw;
            }
        }

        // 💡 Seu mapeador original intacto
        private static ProdutosCadastradosDto MapearParaDto(Guitarras guitarra)
        {
            return new ProdutosCadastradosDto
            {
                Id = guitarra.Id,
                Nome = guitarra.Nome,
                Marca = guitarra.Marca,
                Url_imagem = guitarra.Url_imagem,
                Quantidade = guitarra.Quantidade,
                Preco = (int)guitarra.Preco
            };
        }
    }
}