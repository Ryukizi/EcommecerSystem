namespace Validacao_1.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Validacao_1.Shared.Models;

public class CarrinhoService
{

    // Lista privada que armazena as guitarras adicionadas
    private List<Guitarras> _itens = new List<Guitarras>();

    // Propriedade pública para ler os itens do carrinho de qualquer página
    public IReadOnlyList<Guitarras> Itens => _itens.AsReadOnly();

    public decimal Total => _itens.Sum(g => g.Preco);

    public int QuantidadeTotal => _itens.Count; 

    // Método para adicionar uma guitarra ao carrinho
    public void Adicionar(Guitarras produto)
    {
        _itens.Add(produto);
    }

    public void Remover(long id)
    {
        _itens.RemoveAll(g => g.Id == id);
    }

    // Método para limpar o carrinho após finalizar a compra
    public void LimparCarrinho()
    {
        _itens.Clear();
    }
}
