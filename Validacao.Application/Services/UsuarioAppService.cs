using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Validacao.Application.Interfaces;
using Validacao.Shared.Dtos;
using Validacao.Shared.Models;

namespace Validacao.Aplication.Service
{
    public class UsuarioAppService : IUsuarioAppService
    {
        private readonly ICriptografiaServico _criptografiaServico;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailServico;
        private readonly IConfiguration _config;
        private readonly Supabase.Client _supabaseClient;

        // 💡 Removemos o IValidadorService do construtor do Backend!
        public UsuarioAppService(
            ICriptografiaServico criptografiaServico,
            IUsuarioRepository usuarioRepository,
            IEmailService emailServico,
            IConfiguration config,
            Supabase.Client supabaseClient)
        {
            _criptografiaServico = criptografiaServico;
            _usuarioRepository = usuarioRepository;
            _emailServico = emailServico;
            _config = config;
            _supabaseClient = supabaseClient;
        }

        public async Task<Usuario> CadastrarAsync(UsuarioCadastroDto request)
        {
            // 1. Criamos primeiro a credencial no Supabase Auth
            var cadastroSupabase = await _supabaseClient.Auth.SignUp(request.Email, request.Senha);

            if (cadastroSupabase?.User == null)
            {
                throw new Exception("Falha ao registrar as credenciais de login no serviço de autenticação.");
            }

            // 2. Criptografamos a senha para salvar no banco local
            var senhaCriptografada = _criptografiaServico.HashPassword(request.Senha);

            // 3. Montamos o modelo do usuário (o ID será gerado automaticamente pelo banco local/Supabase)
            var usuario = new Usuario
            {
                Nome = request.Nome,
                Idade = request.Idade,
                Email = request.Email,
                Senha = senhaCriptografada,
                Perfil = "Usuario" // Define o perfil padrão
            };

            // 4. Salva os dados do perfil na tabela local de usuários
            return await _usuarioRepository.InserirAsync(usuario);
        }
        

        public async Task<object> LoginAsync(UsuarioLoginDto request)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não encontrado.");
            }

            if (!_criptografiaServico.VerifyPassword(request.Senha, usuario.Senha))
            {
                throw new UnauthorizedAccessException("Senha incorreta.");
            }

            return new
            {
                Message = "Login bem-sucedido!",
                Usuario = new
                {
                    Id = usuario.Id.ToString(),
                    Nome = usuario.Nome,
                    Email = usuario.Email
                }
            };
        }

        // 💡 Ajustado: Agora consulta o banco diretamente pelo repositório!
        public async Task<string?> ValidarEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "O e-mail não pode ser vazio.";
            }

            try
            {
                // Busca o usuário no banco usando a infraestrutura do próprio backend
                var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);

                if (usuarioExistente != null)
                {
                    return "E-mail já cadastrado no banco de dados";
                }

                return null; // Retorna null se estiver tudo certo (e-mail livre!)
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro interno ao verificar disponibilidade do e-mail: {ex.Message}", ex);
            }
        }

        public async Task<int> EnviarCodigoAsync(string email)
        {
            var codigo = new Random().Next(1000, 10000);

            await _emailServico.EnviarEmailCodigo(email, codigo);

            return codigo;
        }
    }
}