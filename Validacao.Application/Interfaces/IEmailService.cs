namespace Validacao.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailCodigo(string emailDoCliente, int codigo);
    }
}
