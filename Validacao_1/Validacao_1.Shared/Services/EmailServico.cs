using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Validacao_1.Shared.Services
{
    public class EmailServico
    {
        private readonly IConfiguration _config;

        public EmailServico(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmailCodigo(string emailDoCliente, int codigo, string remetente, string usuarioSmtp, string suaSenhaDeApp)
        {
            // 1. Criação da mensagem usando MimeKit
            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("Sistema de Cadastro", remetente));
            mensagem.To.Add(new MailboxAddress("", emailDoCliente));
            mensagem.Subject = "Código de Verificação";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Seu código de verificação é: {codigo}"
            };
            mensagem.Body = bodyBuilder.ToMessageBody();

            // 2. Envio usando o SmtpClient do MailKit (Preste atenção no namespace MailKit.Net.Smtp)
            using var client = new SmtpClient();

            // Conecta usando STARTTLS na porta 587
            await client.ConnectAsync("smtp-relay.brevo.com", 587, SecureSocketOptions.StartTls);

            // Realiza a autenticação explícita exigida pela Brevo
            await client.AuthenticateAsync(usuarioSmtp, suaSenhaDeApp);

            // Envia o e-mail
            await client.SendAsync(mensagem);

            // Desconecta de forma limpa
            await client.DisconnectAsync(true);
        }
    }
}