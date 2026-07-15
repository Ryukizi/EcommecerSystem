using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Validacao.Shared.Models;
using Validacao.Application.Interfaces;

namespace Validacao.Infrastructure.Services
{
    public class ValidadorService : IValidadorService
    {
        private readonly HttpClient _httpClient;

        public ValidadorService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<string>> EntradaDeDados(Pessoa pessoa)
        {
            var erros = new List<string>();

            if (!Idade(pessoa))
            {
                erros.Add("Pessoa é menor de idade, não é possível realizar o cadastro.");
                return erros;
            }

            var erroNome = Nome(pessoa);
            if (!string.IsNullOrEmpty(erroNome)) erros.Add(erroNome);

            var erroEmail = await Email(pessoa);
            if (!string.IsNullOrEmpty(erroEmail)) erros.Add(erroEmail);

            var erroSenha = Senha(pessoa);
            if (!string.IsNullOrEmpty(erroSenha)) erros.Add(erroSenha);

            return erros;
        }

        public bool Idade(Pessoa pessoa)
        {
            return pessoa.Idade >= 18;
        }

        public string Nome(Pessoa pessoa)
        {
            bool validName = !string.IsNullOrWhiteSpace(pessoa.Nome) && pessoa.Nome.All(c => char.IsLetter(c) || c == ' ');
            bool temEspacoDuplicado = Regex.IsMatch(pessoa.Nome ?? string.Empty, @"\s{2,}");

            if (validName && !temEspacoDuplicado)
            {
                return null;
            }
            return "Nome inválido";
        }

        public async Task<string> Email(Pessoa pessoa)
        {
            bool emailValido = Regex.IsMatch(pessoa.Email ?? string.Empty, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailValido)
            {
                return "Formato de e-mail inválido";
            }

            try
            {
                Console.WriteLine($"[VALIDAÇÃO] 📧 Consultando API para verificar se '{pessoa.Email}' já existe...");

                var response = await _httpClient.GetAsync($"api/usuario/verificar-email?email={Uri.EscapeDataString(pessoa.Email)}");

                // 💡 MELHORIA 1: Se a API falhar, capturamos a mensagem exata enviada pelo servidor!
                if (!response.IsSuccessStatusCode)
                {
                    var conteudoErro = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Erro HTTP {(int)response.StatusCode} ({response.StatusCode}): {conteudoErro}");
                }

                var dados = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (dados.TryGetProperty("existe", out var existeProp) && existeProp.GetBoolean())
                {
                    Console.WriteLine("[VALIDAÇÃO] ⚠️ API informou que o e-mail já está cadastrado.");
                    return "E-mail já cadastrado no banco de dados";
                }

                Console.WriteLine("[VALIDAÇÃO] ✅ API confirmou e-mail livre!");
                return null;
            }
            // 💡 MELHORIA 2: O catch agora formata um log detalhado para o seu console
            catch (Exception ex)
            {
                Console.WriteLine($"[VALIDAÇÃO] ❌ ERRO CRÍTICO NA VALIDAÇÃO: {ex}");

                // Relança o erro com a mensagem interna detalhada para estourar na tela do Blazor
                throw new Exception($"Falha na validação do e-mail. Detalhes: {ex.Message}", ex);
            }
        }

        public string Senha(Pessoa pessoa)
        {
            bool senhaValida = Regex.IsMatch(pessoa.Senha ?? string.Empty, @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            if (senhaValida)
            {
                return null;
            }
            return "Senha inválida";
        }
    }
}