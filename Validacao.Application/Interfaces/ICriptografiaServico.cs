namespace Validacao.Application.Interfaces
{
    public interface ICriptografiaServico
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
