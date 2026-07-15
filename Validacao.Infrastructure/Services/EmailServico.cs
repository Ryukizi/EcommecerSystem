using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;
using Validacao.Application.Interfaces; // Importa a interface

namespace Validacao.Infrastructure.Services
{
    public class EmailServico : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailServico(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmailCodigo(string emailDoCliente, int codigo)
        {
            // Busca as credenciais diretamente do appsettings.json
            var remetente = _config["Smtp:Remetente"] ?? "";
            var usuario = _config["Smtp:Usuario"] ?? "";
            var senha = _config["Smtp:Senha"] ?? "";

            // Temporário: Adicione isto para ver se o C# está mesmo achando as configurações!
            if (string.IsNullOrEmpty(remetente) || string.IsNullOrEmpty(usuario))
            {
                throw new Exception("B.O. detectado: As configurações do SMTP estão vindo vazias do appsettings.json!");
            }

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Sistema de Cadastro", remetente));
            mensagem.To.Add(new MailboxAddress("", emailDoCliente));
            mensagem.Subject = "Código de Verificação";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Seu código de verificação é: {codigo}"
            };
            mensagem.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Conecta ao Brevo
            await client.ConnectAsync("smtp-relay.brevo.com", 587, SecureSocketOptions.StartTls);

            // 💡 Autentica usando as variáveis lidas do _config!
            await client.AuthenticateAsync(usuario, senha);

            await client.SendAsync(mensagem);
            await client.DisconnectAsync(true);
        }
    }
}