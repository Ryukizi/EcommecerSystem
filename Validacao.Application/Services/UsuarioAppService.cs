using Microsoft.Extensions.Configuration;
using Validacao.Application.Interfaces;
using Validacao.Application.Interfaces.Repositories;
using Validacao.Shared.Dtos;

namespace Validacao.Aplication.Service
{
    public class UsuarioAppService : IUsuarioAppService
    {
        private readonly ICriptografiaServico _criptografiaServico;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailServico;
        private readonly IConfiguration _config;
        private readonly Supabase.Client _supabaseClient;
        private readonly ITokenService _tokenService;

        // 💡 Removemos o IValidadorService do construtor do Backend!
        public UsuarioAppService(
            ICriptografiaServico criptografiaServico,
            IUsuarioRepository usuarioRepository,
            IEmailService emailServico,
            IConfiguration config,
            Supabase.Client supabaseClient,
            ITokenService tokenService) 
        {
            _criptografiaServico = criptografiaServico;
            _usuarioRepository = usuarioRepository;
            _emailServico = emailServico;
            _config = config;
            _supabaseClient = supabaseClient;
            _tokenService = tokenService;
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

            var accessToken = _tokenService.GerarToken(usuario);
            var refreshToken = _tokenService.GerarRefreshToken();

            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiracao = DateTime.UtcNow.AddDays(7);
            await _usuarioRepository.AtualizarAsync(usuario);

            return new
            {
                Message = "Login bem-sucedido!",
                Token = accessToken,
                RefreshToken = refreshToken,
                Usuario = new
                {
                    Id = usuario.Id.ToString(),
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfil = usuario.Perfil
                }
            };
        }

        public async Task<object> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException("Refresh token inválido.");
            }

            var usuario = await _usuarioRepository.ObterPorRefreshTokenAsync(refreshToken);

            if (usuario == null ||
                string.IsNullOrWhiteSpace(usuario.RefreshToken) ||
                usuario.RefreshToken != refreshToken)
            {
                throw new UnauthorizedAccessException("Refresh token não encontrado ou inválido.");
            }

            if (usuario.RefreshTokenExpiracao.HasValue && usuario.RefreshTokenExpiracao.Value <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expirado.");
            }

            var novoAccessToken = _tokenService.GerarToken(usuario);
            var novoRefreshToken = _tokenService.GerarRefreshToken();

            usuario.RefreshToken = novoRefreshToken;
            usuario.RefreshTokenExpiracao = DateTime.UtcNow.AddDays(7);
            await _usuarioRepository.AtualizarAsync(usuario);

            return new
            {
                Token = novoAccessToken,
                RefreshToken = novoRefreshToken
            };
        }

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
        public async Task<UsuarioLoginDto> ObterPerfilAsync(UsuarioLoginDto request) // 👈 Retorno ajustado para UsuarioLoginDto
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            return new UsuarioLoginDto
            {
                Email = usuario.Email
            };
        }

    }
}