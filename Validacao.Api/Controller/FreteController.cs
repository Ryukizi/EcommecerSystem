using Microsoft.AspNetCore.Mvc;
using Validacao.Application.Interfaces;
using Validacao.Shared.Dtos;

namespace Validacao.Api.Controller
{
    [ApiController]
    [Route("api/cep")]
    public class FreteController : ControllerBase
    {
        private readonly IFreteService _freteService;

        public FreteController(IFreteService freteService)
        {
            _freteService = freteService;
        }

        [HttpGet("{cep}")]
        public async Task<ActionResult<List<FreteDto>>> GetFrete(string cep)
        {
            try
            {
                var frete = await _freteService.CalcularFreteAsync(cep);

                if (frete == null || !frete.Any())
                {
                    return NotFound(new { mensagem = "Frete não encontrado para este CEP." });
                }

                return Ok(frete);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter Frete: {ex.Message}");
            }
        }
    }
}
