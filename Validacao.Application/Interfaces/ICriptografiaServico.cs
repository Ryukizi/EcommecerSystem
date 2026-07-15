using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validacao.Application.Interfaces
{
    public interface ICriptografiaServico
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
