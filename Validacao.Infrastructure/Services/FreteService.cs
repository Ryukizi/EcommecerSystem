using System.Net.Http.Json;
using Validacao.Shared.Dtos;
using Validacao.Infrastructure.Dtos;

namespace Validacao.Infrastructure.Services;

public class FreteService
{
    private readonly HttpClient _httpClient;

    // O .NET vai injetar o HttpClient automaticamente aqui pelo construtor
    public FreteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RespostaFreteDto?> CalcularFreteAsync(string cep)
    {
        try
        {
            string urlViaCep = $"https://viacep.com.br/ws/{cep}/json/";
            var dadosViaCep = await _httpClient.GetFromJsonAsync<ViaCepResponse>(urlViaCep);

            if (dadosViaCep == null || string.IsNullOrEmpty(dadosViaCep.Localidade))
            {
                return null;
            }

            return new RespostaFreteDto
            {
                CepBuscado = cep,
                Cidade = dadosViaCep.Localidade,
                Estado = dadosViaCep.Uf,
                ValorPac = 22.90m,
                PrazoPac = "5 dias úteis",
                ValorSedex = 45.50m,
                PrazoSedex = "2 dias úteis"
            };
        }
        catch
        {
            return null;
        }
    }
}