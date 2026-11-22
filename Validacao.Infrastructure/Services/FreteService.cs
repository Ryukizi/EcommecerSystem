using System.Diagnostics;
using System.Net.Http.Json;
using Validacao.Application.Interfaces;
using Validacao.Infrastructure.Dtos;
using Validacao.Shared.Dtos;

namespace Validacao.Infrastructure.Services;

public class FreteService : IFreteService
{
    private readonly Supabase.Client? _supabase;
    private readonly HttpClient _httpClient;

    public FreteService(
        HttpClient httpClient,
        Supabase.Client? supabase = null)
    {
        _httpClient = httpClient;
        _supabase = supabase;

        Debug.WriteLine("FRETE SERVICE: construtor concluído");
    }

    public async Task<List<FreteDto?>> CalcularFreteAsync(string cep)
    {
        try
        {
            string urlViaCep = $"https://viacep.com.br/ws/{cep}/json/";

            if (_supabase == null)
            {
                return await _httpClient.GetFromJsonAsync<List<FreteDto>>($"{urlViaCep}");
            }

            Debug.WriteLine($"Consultando URL: {urlViaCep}");

            var dadosViaCep =
                await _httpClient.GetFromJsonAsync<ViaCepResponse>(urlViaCep);

            if (dadosViaCep == null ||
                string.IsNullOrEmpty(dadosViaCep.Localidade))
            {
                Debug.WriteLine("ViaCEP retornou nulo.");
                return null;
            }

            Debug.WriteLine($"Buscando UF {dadosViaCep.Uf} no Supabase...");

            var response = await _supabase
                .From<Frete>()
                .Where(f => f.Uf == dadosViaCep.Uf)
                .Get();

            return response.Models
                .Select(MapearParaDto)
                .ToList<FreteDto?>();

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erro ao calcular frete: {ex.Message}");
            return null;
        }
    }
    private static FreteDto MapearParaDto(Frete frete)
    {
        return new FreteDto
        {
            Id = frete.Id,
            Uf = frete.Uf,
            ValorPac = frete.ValorPac,
            PrazoPac = frete.PrazoPac,
            ValorSedex = frete.ValorSedex,
            PrazoSedex = frete.PrazoSedex
        };
    }
}
