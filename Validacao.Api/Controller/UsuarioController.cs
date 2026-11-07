using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Validacao.Application.Interfaces;
using Validacao.Shared.Dtos;

namespace Validacao.Api.Controllers 
{
    [ApiController]
    [Route("api/usuario")] // 
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IConfiguration _configuration;

        public UsuarioController(IUsuarioAppService usuarioAppService, IConfiguration configuration)
        {
            _usuarioAppService = usuarioAppService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Cadastra([FromBody] UsuarioCadastroDto request)
        {
            try
            {
                var usuarioSalvo = await _usuarioAppService.CadastrarAsync(request);

                return Ok(new
                {
                    Id = usuarioSalvo.Id,
                    Nome = usuarioSalvo.Nome,
                    Email = usuarioSalvo.Email,
                    Idade = usuarioSalvo.Idade
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao salvar no banco: {ex.Message}" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto request)
        {
            try
            {
                var resultado = await _usuarioAppService.LoginAsync(request);
                return Ok(resultado);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao tentar logar: {ex.Message}" });
            }
        }


        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> GetPerfil()
        {
            try
            {
                // 1. Extrai o e-mail das Claims do Token JWT
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                // 2. Monta o DTO esperado pelo método
                var dto = new UsuarioLoginDto
                {
                    Email = email
                };

                // 3. Chama o método passando o DTO
                var perfil = await _usuarioAppService.ObterPerfilAsync(dto);
                Console.WriteLine(perfil + "sucesso");
                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
                Console.WriteLine("Sem sucesso");
            }
        }



        // 💡 Rota alterada para "verificar-email" e retorno adaptado para o que o Blazor espera!
        [HttpGet("verificar-email")]
        public async Task<IActionResult> ValidarEmail([FromQuery] string email)
        {
            try
            {
                var erro = await _usuarioAppService.ValidarEmailAsync(email);

                // Se o serviço retornar algum erro de validação (ex: e-mail já existe)
                if (erro != null)
                {
                    return Ok(new { existe = true, mensagem = erro });
                }

                return Ok(new { existe = false, mensagem = "E-mail livre para cadastro!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("enviar-codigo")]
        public async Task<IActionResult> EnviarCodigo([FromBody] EnviarCodigoRequestDto request)
        {
            try
            {
                var codigoGerado = await _usuarioAppService.EnviarCodigoAsync(request.Email);
                return Ok(new { codigoGerado = codigoGerado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao enviar e-mail: {ex.Message}" });
            }
        }
    }
}