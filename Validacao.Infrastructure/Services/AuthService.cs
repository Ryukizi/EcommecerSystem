using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Validacao.Application.Interfaces;
using Validacao.Shared.Dtos;

namespace Validacao.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthStateProvider _authStateProvider;
        private readonly HttpClient _httpClient;

        // 💡 Construtor limpo: o front-end Blazor não precisa mais saber do Supabase!
        public AuthService(
            HttpClient httpClient,
            IAuthStateProvider authStateProvider)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
        }

        public async Task<int> SolicitarCodigoEnvioAsync(string email)
        {
            var payload = new { email = email };
            var response = await _httpClient.PostAsJsonAsync("api/usuario/enviar-codigo", payload);

            if (!response.IsSuccessStatusCode)
            {
                var erroTexto = await response.Content.ReadAsStringAsync();
                throw new Exception(erroTexto);
            }

            var dadosResposta = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (dadosResposta.TryGetProperty("codigoGerado", out var codigoProp))
            {
                return codigoProp.GetInt32();
            }

            throw new Exception("Falha ao ler o código de verificação retornado pela API.");
        }

        public async Task CadastrarUsuarioCompletoAsync(UsuarioCadastroDto usuarioDto)
        {
            // A. Faz uma única chamada para a API (que vai salvar no banco local E criar as credenciais no Supabase)
            var response = await _httpClient.PostAsJsonAsync("api/usuario", usuarioDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(errorContent);
            }

            // B. Lê a resposta contendo o usuário gerado pela API
            var usuarioSalvo = await response.Content.ReadFromJsonAsync<JsonElement>();

            // C. Simula a sessão local com as propriedades necessárias no formato exato que o Provider espera!
            var sessaoSimuladaCadastro = new
            {
                user = new
                {
                    email = usuarioDto.Email,
                    id = usuarioSalvo.GetProperty("id").GetInt64().ToString(), // Usamos o ID numérico convertido em string
                    perfil = "Usuario"
                }
            };

            var sessionJson = JsonSerializer.Serialize(sessaoSimuladaCadastro);
            await _authStateProvider.MarcarComoLogado(sessionJson);
        }

        public async Task<bool> FazerLogin(UsuarioLoginDto usuarioLoginDto)
        {
            var loginDto = new UsuarioLoginDto
            {
                Email = usuarioLoginDto.Email,
                Senha = usuarioLoginDto.Senha,
            };

            var response = await _httpClient.PostAsJsonAsync("api/usuario/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var resultadoJson = await response.Content.ReadAsStringAsync();
                await _authStateProvider.MarcarComoLogado(resultadoJson);
                return true;
            }

            return false;
        }

        public async Task FazerLogout()
        {
            // Como o Supabase não está mais injetado no Web, a invalidação da sessão é puramente local no front
            await _authStateProvider.MarcarComoDeslogado();
        }

        public async Task<ClaimsPrincipal> ObterUsuarioAtual()
        {
            return await _authStateProvider.ObterUsuarioAtualAsync();
        }
    }
}