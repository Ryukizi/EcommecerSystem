namespace Validacao_1.Shared.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Validacao_1.Shared.Models;

public class CarrinhoService
{
    private readonly IJSRuntime _js;

    private List<Guitarras> _itens = new List<Guitarras>();

    private const string CarrinhoStorageKey = "carrinho_v2"; // Mudamos a chave para limpar o histórico antigo sem Nome/Marca

    public CarrinhoService(IJSRuntime js)
    {
        _js = js;
    }

    public IReadOnlyList<Guitarras> Itens => _itens.AsReadOnly();
    public decimal Total => _itens.Sum(g => g.Preco);
    public int QuantidadeTotal => _itens.Count;

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    // 💡 CORREÇÃO 1: Adicionamos Nome e Marca no DTO para salvar no navegador
    private sealed record GuitarraDto(long Id, string Nome, string Marca, decimal Preco, string? url_imagem);

    public async Task CarregarCarrinhoAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string>("localStorage.getItem", CarrinhoStorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var dtoList = JsonSerializer.Deserialize<List<GuitarraDto>>(json, _jsonOptions);
                    if (dtoList != null)
                    {
                        _itens = dtoList.Select(d => new Guitarras
                        {
                            Id = d.Id,
                            Preco = d.Preco,
                            Nome = d.Nome,   // 💡 CORREÇÃO 2: Devolve o Nome para a tela
                            Marca = d.Marca, // 💡 CORREÇÃO 3: Devolve a Marca para a tela
                            Url_imagem = d.url_imagem
                        }).ToList();
                    }
                }
                catch (NotSupportedException nsEx)
                {
                    Console.WriteLine($"Desserialização do carrinho falhou: {nsEx.Message}");
                    _itens = new List<Guitarras>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao desserializar carrinho: {ex.Message}");
                    _itens = new List<Guitarras>();
                }
            }
        }
        catch
        {
            // Prerendering ignora
        }
    }

    private async Task SalvarCarrinhoAsync()
    {
        try
        {
            // 💡 CORREÇÃO 4: Passamos os dados reais de Nome e Marca do produto na hora de salvar
            var dto = _itens.Select(g => new GuitarraDto(g.Id, g.Nome, g.Marca, g.Preco, g.Url_imagem)).ToList();
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            await _js.InvokeVoidAsync("localStorage.setItem", CarrinhoStorageKey, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar no LocalStorage: {ex.Message}");
        }
    }

    public async Task Adicionar(Guitarras produto)
    {
        await CarregarCarrinhoAsync();

        if (!_itens.Any(g => g.Id == produto.Id))
        {
            _itens.Add(produto);
        }

        await SalvarCarrinhoAsync();
    }

    public async Task Remover(long id)
    {
        await CarregarCarrinhoAsync();
        _itens.RemoveAll(g => g.Id == id);
        await SalvarCarrinhoAsync();
    }

    public async Task LimparCarrinho()
    {
        _itens.Clear();
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", CarrinhoStorageKey);
        }
        catch { }
    }
}