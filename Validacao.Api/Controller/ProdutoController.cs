using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Validacao.Application.Interfaces;
using Validacao.Shared.Dtos; 

namespace Validacao.Api.Controllers
{
    [ApiController]
    [Route("api/Produtos")] // Isso define a rota como /api/produto
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        // O ASP.NET injeta o serviço automaticamente aqui graças ao registro que fizemos!
        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProdutosCadastradosDto>>> GetTodos()
        {
            try
            {
                // 1. Chamamos o serviço que criamos na infraestrutura
                var produtos = await _produtoService.GetProdutosAsync();

                // 2. Retornamos a lista com o status HTTP 200 (Ok)
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                // Se algo der errado, retornamos o status 500 (Erro Interno do Servidor)
                return StatusCode(500, $"Erro ao obter produtos: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutosCadastradosDto>> GetPorId(long id)
        {
            try
            {
                // 1. Chamamos o serviço passando o ID recebido na URL
                var produto = await _produtoService.GetProdutoByIdAsync(id);

                // 2. Se o produto não for encontrado, retornamos 404 (Not Found)
                if (produto == null)
                {
                    return NotFound($"Produto com ID {id} não encontrado.");
                }

                // 3. Se tudo der certo, retornamos o produto com 200 (Ok)
                return Ok(produto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter produto: {ex.Message}");
            }
        }

        [HttpGet("buscar")] // 💡 Mudado de "busca" para "buscar" para bater com o Blazor
        public async Task<ActionResult<List<ProdutosCadastradosDto>>> Buscar([FromQuery] string termo) // 💡 Mudado de "busca" para "termo"
        {
            Console.WriteLine($"\n[CONTROLLER] 💻 Requisição HTTP GET recebida em 'api/Produtos/buscar' com parâmetro termo='{termo}'");
            try
            {
                // 1. Chamamos o serviço de busca passando o termo correto
                var produtos = await _produtoService.BuscarGuitarrasAsync(termo ?? "");

                Console.WriteLine($"[CONTROLLER] ✅ Respondendo HTTP 200 OK com {produtos.Count} itens encontrados.");

                // 2. Retornamos a lista de resultados com o status 200 (Ok)
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONTROLLER] ❌ Respondendo HTTP 500 devido a erro: {ex.Message}");
                return StatusCode(500, $"Erro ao buscar produtos: {ex.Message}");
            }
        }
    }
}